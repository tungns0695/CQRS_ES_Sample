using Application.Queries;
using Application.ReadModels;
using Infrastructure.Queries;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.QueryHandlers
{
    public class GetTaxpayerQueryHandler : IConsumer<GetTaxpayerQuery>
    {
        private readonly TaxSystemDbContext _dbContext;
        private readonly ILogger<GetTaxpayerQueryHandler> _logger;

        public GetTaxpayerQueryHandler(TaxSystemDbContext dbContext, ILogger<GetTaxpayerQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetTaxpayerQuery> context)
        {
            try
            {
                var query = context.Message;
                var taxpayer = await _dbContext.Taxpayers
                    .Include(t => t.Addresses)
                    .FirstOrDefaultAsync(t => t.Id == query.TaxpayerId);

                var result = new QueryResult<Taxpayer>
                {
                    Data = taxpayer
                };

                await context.RespondAsync(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetTaxpayerQuery for TaxpayerId: {TaxpayerId}", context.Message.TaxpayerId);
                throw;
            }
        }
    }
}
