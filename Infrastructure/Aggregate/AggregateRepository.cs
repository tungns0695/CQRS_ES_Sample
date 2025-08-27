using Infrastructure.EventStore;
using Infrastructure.Snapshotting;
using Infrastructure.Events;
using Infrastructure.Exceptions;
using MassTransit.Mediator;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Aggregate
{
    public class AggregateRepository<T> : IAggregateRepository<T> where T : IAggregateRoot
    {
        private readonly ISnapshotRepository _snapshotRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEventProjector _eventProjector;
        private readonly int _snapshotThreshold;
        public AggregateRepository(ISnapshotRepository snapshotRepository, IEventRepository eventRepository, IEventProjector eventProjector, IConfiguration configuration)
        {
            _snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
            _snapshotThreshold = configuration.GetValue<int>("SnapshotThreshold");
            _eventProjector = eventProjector;
            _eventRepository = eventRepository;
        }

        public async Task<T> GetAsync(Guid aggregateId, int? expectedVersion = null)
        {
            return await LoadAggregate(aggregateId, expectedVersion);
        }

        public async Task SaveAsync(T aggregate)
        {
            if (aggregate.DomainEvents == null)
            {
                return;
            }

            // Save events to MongoDB (source of truth) and get saved events with positions
            var savedEvents = await _eventRepository.SaveAsync(aggregate);

            // Create snapshot if needed
            if (ShouldMakeSnapshot(aggregate))
            {
                var snapshot = ((dynamic)aggregate).GetSnapshot();
                await _snapshotRepository.SaveAsync(snapshot);
            }

            // Publish events immediately with position information
            await PublishEvents(savedEvents);

            aggregate.ClearDomainEvents();
        }

        private bool ShouldMakeSnapshot(IAggregateRoot aggregate)
        {
            if (aggregate.Version != 0 && aggregate.Version % _snapshotThreshold == 0)
                return true;
            return false;
        }

        public async Task<bool> ExistAsync(Guid aggregateId)
        {
            var events = await _eventRepository.GetEvents(aggregateId);
            return events != null;
        }

        #region private methods
        private async Task<T> LoadAggregate(Guid aggregateId, int? expectedVersion)
        {
            if (expectedVersion < 1)
            {
                throw new AggregateVersionIncorrectException();
            }
            var aggregate = AggregateFactory<T>.CreateAggregate();

            var snapshot = await _snapshotRepository.GetAsync(aggregateId);
            if (snapshot != null)
            {
                var remainingEvents = await _eventRepository.GetEvents(aggregateId, snapshot.Version + 1);
                if (remainingEvents.Any())
                {
                    aggregate.LoadFromHistory(remainingEvents);
                }
                if (expectedVersion != null && aggregate.Version != expectedVersion)
                {
                    throw new ConcurrencyException(aggregateId);
                }
                return aggregate;
            }
            var allEvents = await _eventRepository.GetEvents(aggregateId);
            if (!allEvents.Any())
            {
                throw new AggregateNotFoundException(typeof(T), aggregateId);
            }
            aggregate.LoadFromHistory(allEvents);
            if (expectedVersion != null && aggregate.Version != expectedVersion)
            {
                throw new ConcurrencyException(aggregateId);
            }
            return aggregate;
        }


        private async Task PublishEvents(IEnumerable<StoredEvent> events)
        {
            foreach (var @event in events)
            {
                await _eventProjector.ProcessEventAsync(@event.Event, @event.Position);
            }
        }

        #endregion
    }
}