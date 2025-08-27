using Application.Commands.Taxpayer;
using Application.Domains;
using Application.Domains.Events;
using Infrastructure.Aggregate;
using Infrastructure.Commands;
using MassTransit;
using MongoDB.Bson;

namespace Application.CommandHandlers.Taxpayer
{
    public class UpdateTaxpayerCommandHandler : IConsumer<UpdateTaxpayerCommand>
    {
        private readonly IAggregateRepository<TaxpayerAggregate> _repository;

        public UpdateTaxpayerCommandHandler(IAggregateRepository<TaxpayerAggregate> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<UpdateTaxpayerCommand> context)
        {
            var command = context.Message;
            var taxpayer = await _repository.GetAsync(command.TaxpayerId);
            if (taxpayer == null)
            {
                throw new InvalidOperationException($"Taxpayer with ID {command.TaxpayerId} not found.");
            }

            var @event = new TaxpayerUpdatedEvent
            {
                Id = ObjectId.GenerateNewId().ToString(),
                AggregateId = command.TaxpayerId,
                FirstName = command.FirstName,
                LastName = command.LastName,
                MiddleName = command.MiddleName,
                DateOfBirth = command.DateOfBirth,
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
                LastModifiedBy = command.LastModifiedBy
            };

            taxpayer.ApplyEvent(@event);
            
            await _repository.SaveAsync(taxpayer);
            
            await context.RespondAsync(new CommandResult<bool> { Data = true });
        }
    }
}
