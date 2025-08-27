using Application.Queries;
using Application.ReadModels;
using Infrastructure.Queries;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.QueryHandlers
{
    public class GetTaxpayersWithOutstandingBalanceQueryHandler : IConsumer<GetTaxpayersWithOutstandingBalanceQuery>
    {
        private readonly TaxSystemDbContext _dbContext;
        private readonly ILogger<GetTaxpayersWithOutstandingBalanceQueryHandler> _logger;

        public GetTaxpayersWithOutstandingBalanceQueryHandler(TaxSystemDbContext dbContext, ILogger<GetTaxpayersWithOutstandingBalanceQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetTaxpayersWithOutstandingBalanceQuery> context)
        {
            try
            {
                var taxpayers = await _dbContext.Taxpayers
                    .Include(t => t.Addresses)
                    .Where(t => t.IsActive && t.TaxLiability > t.TaxPaid)
                    .OrderByDescending(t => t.TaxLiability - t.TaxPaid)
                    .ToListAsync();

                var result = new QueryResult<IEnumerable<Taxpayer>>
                {
                    Data = taxpayers
                };

                await context.RespondAsync(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetTaxpayersWithOutstandingBalanceQuery");
                throw;
            }
        }
    }
}
