using Infrastructure.Aggregate;
using MongoDB.Driver;

namespace Infrastructure.Snapshotting
{
    public class SnapshotRepository : ISnapshotRepository
    {
        private readonly IMongoCollection<AggregateRoot> _collection;

        public SnapshotRepository(IMongoDatabase database, string collectionName = "snapshots")
        {
            if (database == null) throw new ArgumentNullException(nameof(database));
            _collection = database.GetCollection<AggregateRoot>(collectionName);

            var indexKeys = Builders<AggregateRoot>.IndexKeys
                .Ascending(x => x.AggregateId)
                .Descending(x => x.Version);

            _collection.Indexes.CreateOne(
                new CreateIndexModel<AggregateRoot>(indexKeys, new CreateIndexOptions { Name = "aggId_version_desc" })
            );
        }

        public async Task<AggregateRoot> GetAsync(Guid aggregateId, CancellationToken ct = default)
        {
            var filter = Builders<AggregateRoot>.Filter.Eq(x => x.AggregateId, aggregateId);
            return await _collection
                .Find(filter)
                .SingleOrDefaultAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task SaveAsync(AggregateRoot snapshot, CancellationToken ct = default)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));
            var filter = Builders<AggregateRoot>.Filter.Eq(x => x.AggregateId, snapshot.AggregateId);
            await _collection.ReplaceOneAsync(
                filter,
                snapshot,
                new ReplaceOptions { IsUpsert = true },
                ct
            ).ConfigureAwait(false);
        }
    }
}