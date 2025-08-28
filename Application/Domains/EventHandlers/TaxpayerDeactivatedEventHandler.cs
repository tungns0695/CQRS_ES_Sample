using MassTransit;
using Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Application.Domains.Events;
using Application.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Application.Domains.EventHandlers
{
    public class TaxpayerDeactivatedEventHandler : IConsumer<TaxpayerDeactivatedEvent>
    {
        private readonly ILogger<TaxpayerDeactivatedEventHandler> _logger;
        private readonly TaxSystemDbContext _dbContext;

        public TaxpayerDeactivatedEventHandler(
            ILogger<TaxpayerDeactivatedEventHandler> logger,
            TaxSystemDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<TaxpayerDeactivatedEvent> context)
        {
            var @event = context.Message;
            _logger.LogInformation("Handling TaxpayerDeactivatedEvent for taxpayer {TaxpayerId}", @event.AggregateId);
            
            try
            {
                var taxpayer = await _dbContext.Taxpayers
                    .FirstOrDefaultAsync(t => t.Id == @event.AggregateId);

                if (taxpayer == null)
                {
                    _logger.LogWarning("Taxpayer read model not found for {TaxpayerId}", @event.AggregateId);
                    return;
                }

                // Deactivate taxpayer
                taxpayer.IsActive = false;
                taxpayer.LastModifiedDate = DateTime.SpecifyKind(@event.OccuredOn.DateTime, DateTimeKind.Utc);
                taxpayer.LastModifiedBy = @event.DeactivatedBy;
                taxpayer.LastEventId = @event.Id;
                taxpayer.Version = @event.Version;

                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Successfully deactivated read model for taxpayer {TaxpayerId}", @event.AggregateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating read model for taxpayer {TaxpayerId}", @event.AggregateId);
                throw;
            }
        }
    }
}
