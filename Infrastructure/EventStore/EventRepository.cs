using Infrastructure.Aggregate;
using Infrastructure.Events;
using Infrastructure.Exceptions;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Reflection;

namespace Infrastructure.EventStore
{
    public class EventRepository : IEventRepository
    {
        private readonly IMongoCollection<StoredEvent> _events;
        private readonly IMongoCollection<BsonDocument> _counters;

        public EventRepository(IMongoDatabase database, string collectionName = "events")
        {
            if (database == null) throw new ArgumentNullException(nameof(database));
            _events = database.GetCollection<StoredEvent>(collectionName);
            _counters = database.GetCollection<BsonDocument>("counters");

            _events.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<StoredEvent>(
                    Builders<StoredEvent>.IndexKeys
                        .Ascending(e => e.AggregateId)
                        .Ascending(e => e.Version),
                    new CreateIndexOptions { Unique = true, Name = "uniq_agg_version" }
                ),
                new CreateIndexModel<StoredEvent>(
                    Builders<StoredEvent>.IndexKeys
                        .Ascending(e => e.AggregateId)
                        .Ascending(e => e.OccuredOn),
                    new CreateIndexOptions { Name = "agg_time" }
                )
            });
        }

        public async Task<IEnumerable<StoredEvent>> SaveAsync(IAggregateRoot aggregate)
        {
            if (aggregate == null) throw new ArgumentNullException(nameof(aggregate));
            if (aggregate.DomainEvents == null || !aggregate.DomainEvents.Any()) return new List<StoredEvent>();

            var aggregateId = aggregate.AggregateId;

            var counterFilter = Builders<BsonDocument>.Filter.Eq("_id", "event_position");
            var counterUpdate = Builders<BsonDocument>.Update.Inc("seq", (long)aggregate.DomainEvents.Count());
            var counterOptions = new FindOneAndUpdateOptions<BsonDocument>
            {
                ReturnDocument = ReturnDocument.Before,
                IsUpsert = true
            };

            var counter = await _counters.FindOneAndUpdateAsync(
                counterFilter,
                counterUpdate,
                counterOptions
            ).ConfigureAwait(false);

            var nextPosition = (counter?.GetValue("seq", 0).AsInt64 ?? 0L) + 1L;

            // Assign consecutive versions to new events.
            var toInsert = aggregate.DomainEvents
                .Select((e, i) => new StoredEvent
                {
                    AggregateId = aggregateId,
                    Version = e.Version,
                    Id = e.Id,
                    Position = nextPosition + i,
                    TypeName = e.GetType().Name,
                    FullName = e.GetType().FullName!,
                    Data = JsonConvert.SerializeObject(e),
                    OccuredOn = DateTime.UtcNow,
                    Event = e
                })
                .ToList();
            
            try
            {
                var opts = new InsertManyOptions { IsOrdered = true };
                await _events.InsertManyAsync(toInsert, opts).ConfigureAwait(false);
                return toInsert;
            }
            catch (MongoWriteException mwx) when (mwx.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new InvalidOperationException("Concurrency conflict while appending events.", mwx);
            }
        }

        public async Task<IEnumerable<IEvent>> GetEvents(Guid aggregateId, int? startVersion = null)
        {
            return await GetEvents(aggregateId, startVersion, CancellationToken.None);
        }

        public async Task<IEnumerable<IEvent>> GetEvents(Guid aggregateId, int? startVersion, CancellationToken ct)
        {
            var filter = Builders<StoredEvent>.Filter.Eq(x => x.AggregateId, aggregateId);

            if (startVersion.HasValue)
            {
                filter &= Builders<StoredEvent>.Filter.Gte(x => x.Version, startVersion.Value);
            }

            var docs = await _events
                .Find(filter)
                .Sort(Builders<StoredEvent>.Sort.Ascending(x => x.Version))
                .ToListAsync(ct)
                .ConfigureAwait(false);

            if (docs == null || docs.Count == 0)
                return Enumerable.Empty<IEvent>();

            var events = new List<IEvent>();
            foreach (var d in docs)
            {
                var applicationAssembly = Assembly.Load("Application");
                var type = applicationAssembly.GetType(d.FullName, throwOnError: true);
                var evt = JsonConvert.DeserializeObject(d.Data, type!) as IEvent;
                if (evt == null)
                {
                    throw new EventDeserializationException($"Event version {d.Version} of aggregate {d.AggregateId} cannot be deserialize");
                }
                events.Add(evt);
            }

            return events;
        }
    }
}