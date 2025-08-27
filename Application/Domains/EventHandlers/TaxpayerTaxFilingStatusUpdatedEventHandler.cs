using MassTransit;
using Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Application.Domains.Events;
using Application.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Application.Domains.EventHandlers
{
    public class TaxpayerTaxFilingStatusUpdatedEventHandler : IConsumer<TaxpayerTaxFilingStatusUpdatedEvent>
    {
        private readonly ILogger<TaxpayerTaxFilingStatusUpdatedEventHandler> _logger;
        private readonly TaxSystemDbContext _dbContext;

        public TaxpayerTaxFilingStatusUpdatedEventHandler(
            ILogger<TaxpayerTaxFilingStatusUpdatedEventHandler> logger,
            TaxSystemDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<TaxpayerTaxFilingStatusUpdatedEvent> context)
        {
            var @event = context.Message;
            _logger.LogInformation("Handling TaxpayerTaxFilingStatusUpdatedEvent for taxpayer {TaxpayerId}", @event.AggregateId);
            
            try
            {
                var taxpayer = await _dbContext.Taxpayers
                    .FirstOrDefaultAsync(t => t.Id == @event.AggregateId);

                if (taxpayer == null)
                {
                    _logger.LogWarning("Taxpayer read model not found for {TaxpayerId}", @event.AggregateId);
                    return;
                }

                // Update tax filing-related properties
                taxpayer.FilingStatus = @event.FilingStatus;
                taxpayer.TaxYear = @event.TaxYear;
                taxpayer.HasFiledTaxes = @event.HasFiledTaxes;
                taxpayer.TaxFilingDate = @event.TaxFilingDate.HasValue 
                    ? DateTime.SpecifyKind(@event.TaxFilingDate.Value, DateTimeKind.Utc) 
                    : null;
                taxpayer.TaxLiability = @event.TaxLiability;
                taxpayer.TaxPaid = @event.TaxPaid;
                taxpayer.TaxRefund = @event.TaxRefund;
                taxpayer.LastModifiedDate = DateTime.SpecifyKind(@event.OccuredOn.DateTime, DateTimeKind.Utc);
                taxpayer.LastModifiedBy = @event.LastModifiedBy;

                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Successfully updated tax filing status read model for taxpayer {TaxpayerId}", @event.AggregateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tax filing status read model for taxpayer {TaxpayerId}", @event.AggregateId);
                throw;
            }
        }
    }
}
