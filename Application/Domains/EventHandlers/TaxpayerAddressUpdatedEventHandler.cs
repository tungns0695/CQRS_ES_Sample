using MassTransit;
using Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Application.Domains.Events;
using Application.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Application.Domains.EventHandlers
{
    public class TaxpayerAddressUpdatedEventHandler : IConsumer<TaxpayerAddressUpdatedEvent>
    {
        private readonly ILogger<TaxpayerAddressUpdatedEventHandler> _logger;
        private readonly TaxSystemDbContext _dbContext;

        public TaxpayerAddressUpdatedEventHandler(
            ILogger<TaxpayerAddressUpdatedEventHandler> logger,
            TaxSystemDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<TaxpayerAddressUpdatedEvent> context)
        {
            var @event = context.Message;
            _logger.LogInformation("Handling TaxpayerAddressUpdatedEvent for taxpayer {TaxpayerId}", @event.AggregateId);
            
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

                // Update address properties
                address.StreetAddress = @event.Address.StreetAddress;
                address.City = @event.Address.City;
                address.State = @event.Address.State;
                address.ZipCode = @event.Address.ZipCode;
                address.Country = @event.Address.Country;
                address.AddressType = @event.Address.AddressType;
                address.IsPrimary = @event.Address.IsPrimary;
                address.LastModifiedDate = @event.Address.LastModifiedDate.HasValue 
                    ? DateTime.SpecifyKind(@event.Address.LastModifiedDate.Value, DateTimeKind.Utc) 
                    : null;

                // Update taxpayer's last modified date
                var taxpayer = await _dbContext.Taxpayers
                    .FirstOrDefaultAsync(t => t.Id == @event.AggregateId);
                
                if (taxpayer != null)
                {
                    taxpayer.LastModifiedDate = DateTime.SpecifyKind(@event.OccuredOn.DateTime, DateTimeKind.Utc);
                    taxpayer.LastModifiedBy = @event.UpdatedBy;
                }

                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Successfully updated address read model for taxpayer {TaxpayerId}", @event.AggregateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address read model for taxpayer {TaxpayerId}", @event.AggregateId);
                throw;
            }
        }
    }
}
