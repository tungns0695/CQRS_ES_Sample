using MassTransit;
using Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Application.Domains.Events;
using Application.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Application.Domains.EventHandlers
{
    public class TaxpayerAddressRemovedEventHandler : IConsumer<TaxpayerAddressRemovedEvent>
    {
        private readonly ILogger<TaxpayerAddressRemovedEventHandler> _logger;
        private readonly TaxSystemDbContext _dbContext;

        public TaxpayerAddressRemovedEventHandler(
            ILogger<TaxpayerAddressRemovedEventHandler> logger,
            TaxSystemDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<TaxpayerAddressRemovedEvent> context)
        {
            var @event = context.Message;
            _logger.LogInformation("Handling TaxpayerAddressRemovedEvent for taxpayer {TaxpayerId}", @event.AggregateId);
            
            try
            {
                var address = await _dbContext.TaxpayerAddresses
                    .FirstOrDefaultAsync(a => a.Id == @event.AddressId && a.TaxpayerId == @event.AggregateId);

                if (address == null)
                {
                    _logger.LogWarning("Address read model not found for address {AddressId} and taxpayer {TaxpayerId}", 
                        @event.AddressId, @event.AggregateId);
                    return;
                }

                // Remove address
                _dbContext.TaxpayerAddresses.Remove(address);

                // Update taxpayer's last modified date
                var taxpayer = await _dbContext.Taxpayers
                    .FirstOrDefaultAsync(t => t.Id == @event.AggregateId);
                
                if (taxpayer != null)
                {
                    taxpayer.LastModifiedDate = DateTime.SpecifyKind(@event.OccuredOn.DateTime, DateTimeKind.Utc);
                    taxpayer.LastModifiedBy = @event.RemovedBy;
                }

                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Successfully removed address read model for taxpayer {TaxpayerId}", @event.AggregateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing address read model for taxpayer {TaxpayerId}", @event.AggregateId);
                throw;
            }
        }
    }
}
