using Infrastructure.Events;

namespace Infrastructure.Events
{
    public interface IEventStoreReader
    {
        Task<IEnumerable<StoredEvent>> GetEventsFromPositionAsync(long fromPosition, int batchSize = 100);
        Task<long> GetLastEventPositionAsync();
        Task<IEnumerable<StoredEvent>> GetEventsForAggregateAsync(Guid aggregateId, long? fromVersion = null);
    }
}

