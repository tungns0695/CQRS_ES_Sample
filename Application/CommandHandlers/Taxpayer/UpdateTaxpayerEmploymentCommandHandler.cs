using Application.Commands.Taxpayer;
using Application.Domains;
using Application.Domains.Events;
using Infrastructure.Aggregate;
using Infrastructure.Commands;
using MassTransit;
using MongoDB.Bson;

namespace Application.CommandHandlers.Taxpayer
{
    public class UpdateTaxpayerEmploymentCommandHandler : IConsumer<UpdateTaxpayerEmploymentCommand>
    {
        private readonly IAggregateRepository<TaxpayerAggregate> _repository;

        public UpdateTaxpayerEmploymentCommandHandler(IAggregateRepository<TaxpayerAggregate> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<UpdateTaxpayerEmploymentCommand> context)
        {
            var command = context.Message;
            var taxpayer = await _repository.GetAsync(command.TaxpayerId);
            if (taxpayer == null)
            {
                throw new InvalidOperationException($"Taxpayer with ID {command.TaxpayerId} not found.");
            }

            var @event = new TaxpayerEmploymentUpdatedEvent
            {
                Id = ObjectId.GenerateNewId().ToString(),
                AggregateId = command.TaxpayerId,
                EmploymentStatus = command.EmploymentStatus,
                EmployerName = command.EmployerName,
                EmployerIdentificationNumber = command.EmployerIdentificationNumber,
                AnnualIncome = command.AnnualIncome,
                LastModifiedBy = command.LastModifiedBy
            };

            taxpayer.ApplyEvent(@event);
            
            await _repository.SaveAsync(taxpayer);
            
            await context.RespondAsync(new CommandResult<bool> { Data = true });
        }
    }
}
