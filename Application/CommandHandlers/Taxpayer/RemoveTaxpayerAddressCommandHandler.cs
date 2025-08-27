using Application.Commands.Taxpayer;
using Application.Domains;
using Application.Domains.Events;
using Infrastructure.Aggregate;
using Infrastructure.Commands;
using MassTransit;
using MongoDB.Bson;

namespace Application.CommandHandlers.Taxpayer
{
    public class RemoveTaxpayerAddressCommandHandler : IConsumer<RemoveTaxpayerAddressCommand>
    {
        private readonly IAggregateRepository<TaxpayerAggregate> _repository;

        public RemoveTaxpayerAddressCommandHandler(IAggregateRepository<TaxpayerAggregate> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<RemoveTaxpayerAddressCommand> context)
        {
            var command = context.Message;
            var taxpayer = await _repository.GetAsync(command.TaxpayerId);
            if (taxpayer == null)
            {
                throw new InvalidOperationException($"Taxpayer with ID {command.TaxpayerId} not found.");
            }

            var @event = new TaxpayerAddressRemovedEvent
            {
                Id = ObjectId.GenerateNewId().ToString(),
                AggregateId = command.TaxpayerId,
                AddressId = command.AddressId,
                RemovedBy = command.RemovedBy
            };

            taxpayer.ApplyEvent(@event);
            
            await _repository.SaveAsync(taxpayer);
            
            await context.RespondAsync(new CommandResult<bool> { Data = true });
        }
    }
}
