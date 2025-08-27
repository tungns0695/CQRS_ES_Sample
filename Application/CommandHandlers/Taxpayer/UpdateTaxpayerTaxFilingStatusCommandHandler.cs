using Application.Commands.Taxpayer;
using Application.Domains;
using Application.Domains.Events;
using Infrastructure.Aggregate;
using Infrastructure.Commands;
using MassTransit;
using MongoDB.Bson;

namespace Application.CommandHandlers.Taxpayer
{
    public class UpdateTaxpayerTaxFilingStatusCommandHandler : IConsumer<UpdateTaxpayerTaxFilingStatusCommand>
    {
        private readonly IAggregateRepository<TaxpayerAggregate> _repository;

        public UpdateTaxpayerTaxFilingStatusCommandHandler(IAggregateRepository<TaxpayerAggregate> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<UpdateTaxpayerTaxFilingStatusCommand> context)
        {
            var command = context.Message;
            var taxpayer = await _repository.GetAsync(command.TaxpayerId);
            if (taxpayer == null)
            {
                throw new InvalidOperationException($"Taxpayer with ID {command.TaxpayerId} not found.");
            }

            var @event = new TaxpayerTaxFilingStatusUpdatedEvent
            {
                Id = ObjectId.GenerateNewId().ToString(),
                AggregateId = command.TaxpayerId,
                FilingStatus = command.FilingStatus,
                TaxYear = command.TaxYear,
                HasFiledTaxes = command.HasFiledTaxes,
                TaxFilingDate = command.TaxFilingDate,
                TaxLiability = command.TaxLiability,
                TaxPaid = command.TaxPaid,
                TaxRefund = command.TaxRefund,
                LastModifiedBy = command.LastModifiedBy
            };

            taxpayer.ApplyEvent(@event);
            
            await _repository.SaveAsync(taxpayer);
            
            await context.RespondAsync(new CommandResult<bool> { Data = true });
        }
    }
}
