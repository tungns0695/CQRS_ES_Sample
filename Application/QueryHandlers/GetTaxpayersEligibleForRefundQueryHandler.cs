using Application.Queries;
using Application.ReadModels;
using Infrastructure.Queries;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.QueryHandlers
{
    public class GetTaxpayersEligibleForRefundQueryHandler : IConsumer<GetTaxpayersEligibleForRefundQuery>
    {
        private readonly TaxSystemDbContext _dbContext;
        private readonly ILogger<GetTaxpayersEligibleForRefundQueryHandler> _logger;

        public GetTaxpayersEligibleForRefundQueryHandler(TaxSystemDbContext dbContext, ILogger<GetTaxpayersEligibleForRefundQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetTaxpayersEligibleForRefundQuery> context)
        {
            try
            {
                var taxpayers = await _dbContext.Taxpayers
                    .Include(t => t.Addresses)
                    .Where(t => t.IsActive && t.TaxPaid > t.TaxLiability)
                    .OrderByDescending(t => t.TaxPaid - t.TaxLiability)
                    .ToListAsync();

                var result = new QueryResult<IEnumerable<Taxpayer>>
                {
                    Data = taxpayers
                };

                await context.RespondAsync(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetTaxpayersEligibleForRefundQuery");
                throw;
            }
        }
    }
}
