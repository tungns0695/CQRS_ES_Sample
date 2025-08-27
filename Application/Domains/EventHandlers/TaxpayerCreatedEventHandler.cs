using MassTransit;
using Microsoft.Extensions.Logging;
using Application.Domains.Events;
using Application.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Application.Domains.EventHandlers
{
    public class TaxpayerCreatedEventHandler : IConsumer<TaxpayerCreatedEvent>
    {
        private readonly ILogger<TaxpayerCreatedEventHandler> _logger;
        private readonly TaxSystemDbContext _dbContext;

        public TaxpayerCreatedEventHandler(
            ILogger<TaxpayerCreatedEventHandler> logger,
            TaxSystemDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<TaxpayerCreatedEvent> context)
        {
            var @event = context.Message;
            _logger.LogInformation("Handling TaxpayerCreatedEvent for taxpayer {TaxpayerId}", @event.AggregateId);
            
            try
            {
                // Create new taxpayer read model
                var taxpayer = new Taxpayer
                {
                    Id = @event.AggregateId,
                    FirstName = @event.FirstName,
                    LastName = @event.LastName,
                    MiddleName = @event.MiddleName,
                    DateOfBirth = DateTime.SpecifyKind(@event.DateOfBirth, DateTimeKind.Utc),
                    SocialSecurityNumber = @event.SocialSecurityNumber,
                    TaxIdentificationNumber = @event.TaxIdentificationNumber,
                    Email = @event.Email,
                    PhoneNumber = @event.PhoneNumber,
                    IsActive = true,
                    CreatedDate = DateTime.SpecifyKind(@event.OccuredOn.DateTime, DateTimeKind.Utc),
                    CreatedBy = @event.CreatedBy,
                    LastModifiedDate = DateTime.SpecifyKind(@event.OccuredOn.DateTime, DateTimeKind.Utc),
                    LastModifiedBy = @event.CreatedBy,
                    Version = @event.Version,
                    LastEventId = @event.Id,
                    LastEventPosition = @event.Position
                };

                // Add addresses if any
                if (@event.Addresses != null && @event.Addresses.Any())
                {
                    taxpayer.Addresses = @event.Addresses.Select(a => new TaxpayerAddress
                    {
                        Id = a.Id,
                        TaxpayerId = @event.AggregateId,
                        StreetAddress = a.StreetAddress,
                        City = a.City,
                        State = a.State,
                        ZipCode = a.ZipCode,
                        Country = a.Country,
                        AddressType = a.AddressType,
                        IsPrimary = a.IsPrimary,
                        CreatedDate = DateTime.SpecifyKind(a.CreatedDate, DateTimeKind.Utc),
                        LastModifiedDate = a.LastModifiedDate.HasValue 
                            ? DateTime.SpecifyKind(a.LastModifiedDate.Value, DateTimeKind.Utc) 
                            : null,
                        Version = @event.Version,
                        LastEventId = @event.Id,
                        LastEventPosition = @event.Position
                    }).ToList();
                }

                _dbContext.Taxpayers.Add(taxpayer);
                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Successfully created read model for taxpayer {TaxpayerId}", @event.AggregateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating read model for taxpayer {TaxpayerId}", @event.AggregateId);
                throw;
            }
        }
    }
}
