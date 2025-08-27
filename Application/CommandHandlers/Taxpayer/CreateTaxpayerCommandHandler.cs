using Application.Commands.Taxpayer;
using Application.Domains;
using Application.Domains.Events;
using Infrastructure.Aggregate;
using Infrastructure.Commands;
using MassTransit;
using MongoDB.Bson;

namespace Application.CommandHandlers.Taxpayer
{
    public class CreateTaxpayerCommandHandler : IConsumer<CreateTaxpayerCommand>
    {
        private readonly IAggregateRepository<TaxpayerAggregate> _repository;

        public CreateTaxpayerCommandHandler(IAggregateRepository<TaxpayerAggregate> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<CreateTaxpayerCommand> context)
        {
            var command = context.Message;
            var taxpayerId = Guid.NewGuid();
            
            var taxpayer = new TaxpayerAggregate();
            
            var @event = new TaxpayerCreatedEvent
            {
                Id = ObjectId.GenerateNewId().ToString(),
                AggregateId = taxpayerId,
                FirstName = command.FirstName,
                LastName = command.LastName,
                MiddleName = command.MiddleName,
                DateOfBirth = command.DateOfBirth,
                SocialSecurityNumber = command.SocialSecurityNumber,
                TaxIdentificationNumber = command.TaxIdentificationNumber,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                Addresses = command.Addresses.Select(a => new Address(
                    Guid.NewGuid(),
                    a.StreetAddress,
                    a.City,
                    a.State,
                    a.ZipCode,
                    a.Country,
                    a.AddressType,
                    a.IsPrimary
                )).ToList(),
                CreatedBy = command.CreatedBy
            };

            taxpayer.ApplyEvent(@event);
            
            await _repository.SaveAsync(taxpayer);
            
            await context.RespondAsync(new CommandResult<Guid> { Data = taxpayerId });
        }
    }
}
