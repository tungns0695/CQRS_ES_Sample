using Application.Commands.Taxpayer;
using Application.Domains;
using Application.Domains.Events;
using Infrastructure.Aggregate;
using Infrastructure.Commands;
using MassTransit;
using MongoDB.Bson;

namespace Application.CommandHandlers.Taxpayer
{
    public class AddTaxpayerAddressCommandHandler : IConsumer<AddTaxpayerAddressCommand>
    {
        private readonly IAggregateRepository<TaxpayerAggregate> _repository;

        public AddTaxpayerAddressCommandHandler(IAggregateRepository<TaxpayerAggregate> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<AddTaxpayerAddressCommand> context)
        {
            var command = context.Message;
            var taxpayer = await _repository.GetAsync(command.TaxpayerId);
            if (taxpayer == null)
            {
                throw new InvalidOperationException($"Taxpayer with ID {command.TaxpayerId} not found.");
            }

            var @event = new TaxpayerAddressAddedEvent
            {
                Id = ObjectId.GenerateNewId().ToString(),
                AggregateId = command.TaxpayerId,
                Address = new Address(
                    Guid.NewGuid(),
                    command.Address.StreetAddress,
                    command.Address.City,
                    command.Address.State,
                    command.Address.ZipCode,
                    command.Address.Country,
                    command.Address.AddressType,
                    command.Address.IsPrimary
                ),
                AddedBy = command.AddedBy
            };

            taxpayer.ApplyEvent(@event);
            
            await _repository.SaveAsync(taxpayer);
            
            await context.RespondAsync(new CommandResult<bool> { Data = true });
        }
    }
}
