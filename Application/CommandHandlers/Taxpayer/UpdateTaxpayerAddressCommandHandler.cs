using Application.Commands.Taxpayer;
using Application.Domains;
using Application.Domains.Events;
using Infrastructure.Aggregate;
using Infrastructure.Commands;
using MassTransit;
using MongoDB.Bson;

namespace Application.CommandHandlers.Taxpayer
{
    public class UpdateTaxpayerAddressCommandHandler : IConsumer<UpdateTaxpayerAddressCommand>
    {
        private readonly IAggregateRepository<TaxpayerAggregate> _repository;

        public UpdateTaxpayerAddressCommandHandler(IAggregateRepository<TaxpayerAggregate> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<UpdateTaxpayerAddressCommand> context)
        {
            var command = context.Message;
            var taxpayer = await _repository.GetAsync(command.TaxpayerId);
            if (taxpayer == null)
            {
                throw new InvalidOperationException($"Taxpayer with ID {command.TaxpayerId} not found.");
            }

            var @event = new TaxpayerAddressUpdatedEvent
            {
                Id = ObjectId.GenerateNewId().ToString(),
                AggregateId = command.TaxpayerId,
                AddressId = command.AddressId,
                Address = new Address(
                    command.AddressId, // Use the existing address ID for updates
                    command.Address.StreetAddress,
                    command.Address.City,
                    command.Address.State,
                    command.Address.ZipCode,
                    command.Address.Country,
                    command.Address.AddressType,
                    command.Address.IsPrimary
                ),
                UpdatedBy = command.UpdatedBy
            };

            taxpayer.ApplyEvent(@event);
            
            await _repository.SaveAsync(taxpayer);
            
            await context.RespondAsync(new CommandResult<bool> { Data = true });
        }
    }
}
