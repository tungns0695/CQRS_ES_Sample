using Infrastructure.Events;
using Infrastructure.Exceptions;
using MongoDB.Bson;

namespace Infrastructure.Aggregate
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        private List<IEvent> _domainEvents = new List<IEvent>();
        public IReadOnlyCollection<IEvent> DomainEvents => _domainEvents?.AsReadOnly() ?? new List<IEvent>().AsReadOnly();

        public Guid AggregateId { get; protected set; }
        public int Version { get; protected set; }

        public void ApplyEvent(IEvent @event)
        {
            lock (_domainEvents)
            {
                @event.Id = ObjectId.GenerateNewId().ToString();
                @event.Version = Version + 1;
                Mutate(@event);
                @event.AggregateId = AggregateId;
                @event.OccuredOn = DateTimeOffset.UtcNow;
                _domainEvents.Add(@event);
            }
        }

        public abstract void Mutate(IEvent @event);

        public void LoadFromHistory(IEnumerable<IEvent> history)
        {
            lock (_domainEvents)
            {
                foreach (var e in history.ToArray())
                {
                    if (e.Version != Version + 1)
                    {
                        throw new AggregateOrEventMissingIdException(GetType(), e.GetType());
                    }
                    if (e.AggregateId != AggregateId && AggregateId != default)
                    {
                        throw new EventIdIncorrectException(e.AggregateId, AggregateId);
                    }
                    ((dynamic)this).When((dynamic)e);
                    AggregateId = e.AggregateId;
                    Version++;
                }
            }
        }

        public void ClearDomainEvents()
        {
            lock (_domainEvents)
            {
                if (DomainEvents == null)
                {
                    return;
                }
                _domainEvents.Clear();
            }
        }
    }
}