using Infrastructure.Aggregate;
using Infrastructure.Events;

namespace Infrastructure.EventStore
{
    public interface IEventRepository
    {
        Task<IEnumerable<StoredEvent>> SaveAsync(IAggregateRoot aggregate);

        Task<IEnumerable<IEvent>> GetEvents(Guid aggregateId, int? startVersion = null);
    }
}