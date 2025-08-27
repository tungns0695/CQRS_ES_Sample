using MassTransit;
using Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Application.Domains.Events;
using Application.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Application.Domains.EventHandlers
{
    public class TaxpayerAddressAddedEventHandler : IConsumer<TaxpayerAddressAddedEvent>
    {
        private readonly ILogger<TaxpayerAddressAddedEventHandler> _logger;
        private readonly TaxSystemDbContext _dbContext;

        public TaxpayerAddressAddedEventHandler(
            ILogger<TaxpayerAddressAddedEventHandler> logger,
            TaxSystemDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<TaxpayerAddressAddedEvent> context)
        {
            var @event = context.Message;
            _logger.LogInformation("Handling TaxpayerAddressAddedEvent for taxpayer {TaxpayerId}", @event.AggregateId);
            
            try
            {
                // Create new address read model
                var address = new TaxpayerAddress
                {
                    Id = @event.Address.Id,
                    TaxpayerId = @event.AggregateId,
                    StreetAddress = @event.Address.StreetAddress,
                    City = @event.Address.City,
                    State = @event.Address.State,
                    ZipCode = @event.Address.ZipCode,
                    Country = @event.Address.Country,
                    AddressType = @event.Address.AddressType,
                    IsPrimary = @event.Address.IsPrimary,
                    CreatedDate = DateTime.SpecifyKind(@event.Address.CreatedDate, DateTimeKind.Utc),
                    LastModifiedDate = @event.Address.LastModifiedDate.HasValue 
                        ? DateTime.SpecifyKind(@event.Address.LastModifiedDate.Value, DateTimeKind.Utc) 
                        : null
                };

                _dbContext.TaxpayerAddresses.Add(address);

                // Update taxpayer's last modified date
                var taxpayer = await _dbContext.Taxpayers
                    .FirstOrDefaultAsync(t => t.Id == @event.AggregateId);
                
                if (taxpayer != null)
                {
                    taxpayer.LastModifiedDate = DateTime.SpecifyKind(@event.OccuredOn.DateTime, DateTimeKind.Utc);
                    taxpayer.LastModifiedBy = @event.AddedBy;
                }

                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Successfully added address read model for taxpayer {TaxpayerId}", @event.AggregateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding address read model for taxpayer {TaxpayerId}", @event.AggregateId);
                throw;
            }
        }
    }
}
