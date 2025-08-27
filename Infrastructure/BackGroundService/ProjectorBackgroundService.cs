using Infrastructure.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;

namespace Infrastructure.BackGroundService
{
    public class ProjectorBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ProjectorBackgroundService> _logger;
        private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(5);
        private readonly int _batchSize = 50;

        public ProjectorBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<ProjectorBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //_logger.LogInformation("Projector background service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessEventsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing events in projector background service");
                }

                await Task.Delay(_pollingInterval, stoppingToken);
            }

            //_logger.LogInformation("Projector background service stopped");
        }

        private async Task ProcessEventsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var eventStoreReader = scope.ServiceProvider.GetRequiredService<IEventStoreReader>();
            var projectors = scope.ServiceProvider.GetServices<IEventProjector>();

            foreach (var projector in projectors)
            {
                try
                {
                    await ProcessEventsForProjectorAsync(projector, eventStoreReader, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing events for projector {ProjectorName}", projector.ProjectorName);
                }
            }
        }

        private async Task ProcessEventsForProjectorAsync(
            IEventProjector projector, 
            IEventStoreReader eventStoreReader, 
            CancellationToken cancellationToken)
        {
            var lastPosition = await projector.GetLastProcessedPositionAsync();
            var events = await eventStoreReader.GetEventsFromPositionAsync(lastPosition + 1, _batchSize);

            foreach (var storedEvent in events)
            {
                try
                {
                    var @event = DeserializeEvent(storedEvent);
                    await projector.ProcessEventAsync(@event, storedEvent.Position);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process event {EventId} at position {Position} for projector {ProjectorName}", 
                        storedEvent.Id, storedEvent.Position, projector.ProjectorName);
                    
                    // TODO: Send to Dead Letter Queue
                    // For now, we'll continue processing other events
                }
            }
        }

        private IEvent DeserializeEvent(StoredEvent storedEvent)
        {
            var eventType = Type.GetType(storedEvent.FullName);
            if (eventType == null)
            {
                // Try to load the type from the Application assembly without direct reference
                var applicationAssembly = Assembly.Load("Application");
                eventType = applicationAssembly.GetType(storedEvent.FullName);
            }
            
            if (eventType == null)
            {
                throw new InvalidOperationException($"Cannot find event type: {storedEvent.FullName}");
            }

            var @event = JsonConvert.DeserializeObject(storedEvent.Data, eventType) as IEvent;
            if (@event == null)
            {
                throw new InvalidOperationException($"Failed to deserialize event: {storedEvent.Id}");
            }

            // Set the event properties
            @event.Id = storedEvent.Id;
            @event.AggregateId = storedEvent.AggregateId;
            @event.Version = storedEvent.Version;
            @event.Position = storedEvent.Position;
            @event.OccuredOn = storedEvent.OccuredOn;

            return @event;
        }
    }
}
