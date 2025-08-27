using MassTransit;
using Microsoft.Extensions.Logging;
using Application.Domains.Events;
using Microsoft.EntityFrameworkCore;

namespace Application.Domains.EventHandlers
{
    public class TaxpayerUpdatedEventHandler : IConsumer<TaxpayerUpdatedEvent>
    {
        private readonly ILogger<TaxpayerUpdatedEventHandler> _logger;
        private readonly TaxSystemDbContext _dbContext;

        public TaxpayerUpdatedEventHandler(
            ILogger<TaxpayerUpdatedEventHandler> logger,
            TaxSystemDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<TaxpayerUpdatedEvent> context)
        {
            var @event = context.Message;
            _logger.LogInformation("Handling TaxpayerUpdatedEvent for taxpayer {TaxpayerId}", @event.AggregateId);
            
            try
            {
                var taxpayer = await _dbContext.Taxpayers
                    .FirstOrDefaultAsync(t => t.Id == @event.AggregateId);

                if (taxpayer == null)
                {
                    _logger.LogWarning("Taxpayer read model not found for {TaxpayerId}", @event.AggregateId);
                    return;
                }

                // Update taxpayer properties
                taxpayer.FirstName = @event.FirstName;
                taxpayer.LastName = @event.LastName;
                taxpayer.MiddleName = @event.MiddleName;
                taxpayer.DateOfBirth = DateTime.SpecifyKind(@event.DateOfBirth, DateTimeKind.Utc);
                taxpayer.Email = @event.Email;
                taxpayer.PhoneNumber = @event.PhoneNumber;
                taxpayer.LastModifiedDate = DateTime.SpecifyKind(@event.OccuredOn.DateTime, DateTimeKind.Utc);
                taxpayer.LastModifiedBy = @event.LastModifiedBy;
                taxpayer.Version = @event.Version;
                taxpayer.LastEventId = @event.Id;
                taxpayer.LastEventPosition = @event.Position;

                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Successfully updated read model for taxpayer {TaxpayerId}", @event.AggregateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating read model for taxpayer {TaxpayerId}", @event.AggregateId);
                throw;
            }
        }
    }
}
