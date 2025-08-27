using Infrastructure.Events;

namespace Infrastructure.Aggregate
{
    public interface IAggregateRoot
    {
        Guid AggregateId { get; }
        int Version { get; }
        IReadOnlyCollection<IEvent> DomainEvents { get; }

        void ApplyEvent(IEvent @event);

        void LoadFromHistory(IEnumerable<IEvent> history);

        void ClearDomainEvents();
    }
}