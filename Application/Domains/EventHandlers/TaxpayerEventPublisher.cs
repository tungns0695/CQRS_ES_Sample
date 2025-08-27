using Infrastructure.Events;
using Microsoft.Extensions.Logging;
using MassTransit.Mediator;

namespace Application.Domains.EventHandlers
{
    public class TaxpayerEventPublisher : BaseEventProjector
    {
        private readonly IScopedMediator _mediator;

        public TaxpayerEventPublisher(
            IProjectorDbContext dbContext, 
            ILogger<BaseEventProjector> logger,
            IScopedMediator publishEndpoint) 
            : base(dbContext, logger)
        {
            _mediator = publishEndpoint;
        }

        public override string ProjectorName => "TaxpayerEventPublisher";

        protected override async Task ProcessEventInternalAsync(IEvent @event, long position)
        {
            _logger.LogInformation("Publishing event {EventId} of type {EventType} for aggregate {AggregateId}", 
                @event.Id, @event.GetType().Name, @event.AggregateId);

            try
            {
                await _mediator.Publish(@event);
                
                _logger.LogInformation("Successfully published event {EventId} to MassTransit", @event.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish event {EventId} to MassTransit", @event.Id);
                throw;
            }
        }
    }
}
