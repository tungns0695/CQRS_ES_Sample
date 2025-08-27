using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Events
{
    public class EventStoreReader : IEventStoreReader
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<EventStoreReader> _logger;
        private readonly IMongoCollection<StoredEvent> _events;

        public EventStoreReader(IMongoDatabase database, ILogger<EventStoreReader> logger)
        {
            _database = database;
            _logger = logger;
            _events = database.GetCollection<StoredEvent>("events");
        }

        public async Task<IEnumerable<StoredEvent>> GetEventsFromPositionAsync(long fromPosition, int batchSize = 100)
        {
            var filter = Builders<StoredEvent>.Filter.Gte(e => e.Position, fromPosition);
            var sort = Builders<StoredEvent>.Sort.Ascending(e => e.Position);

            var events = await _events
                .Find(filter)
                .Sort(sort)
                .Limit(batchSize)
                .ToListAsync();

            _logger.LogDebug("Retrieved {Count} events from position {FromPosition}", events.Count, fromPosition);
            return events;
        }

        public async Task<long> GetLastEventPositionAsync()
        {
            var sort = Builders<StoredEvent>.Sort.Descending(e => e.Position);
            var lastEvent = await _events
                .Find(Builders<StoredEvent>.Filter.Empty)
                .Sort(sort)
                .Limit(1)
                .FirstOrDefaultAsync();

            return lastEvent?.Position ?? 0;
        }

        public async Task<IEnumerable<StoredEvent>> GetEventsForAggregateAsync(Guid aggregateId, long? fromVersion = null)
        {
            var filter = Builders<StoredEvent>.Filter.Eq(e => e.AggregateId, aggregateId);
            
            if (fromVersion.HasValue)
            {
                filter &= Builders<StoredEvent>.Filter.Gte(e => e.Version, fromVersion.Value);
            }

            var sort = Builders<StoredEvent>.Sort.Ascending(e => e.Version);

            var events = await _events
                .Find(filter)
                .Sort(sort)
                .ToListAsync();

            _logger.LogDebug("Retrieved {Count} events for aggregate {AggregateId} from version {FromVersion}", 
                events.Count, aggregateId, fromVersion);

            return events;
        }
    }
}

