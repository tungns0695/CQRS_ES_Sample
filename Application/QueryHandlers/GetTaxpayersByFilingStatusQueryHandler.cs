using Application.Queries;
using Application.ReadModels;
using Infrastructure.Queries;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.QueryHandlers
{
    public class GetTaxpayersByFilingStatusQueryHandler : IConsumer<GetTaxpayersByFilingStatusQuery>
    {
        private readonly TaxSystemDbContext _dbContext;
        private readonly ILogger<GetTaxpayersByFilingStatusQueryHandler> _logger;

        public GetTaxpayersByFilingStatusQueryHandler(TaxSystemDbContext dbContext, ILogger<GetTaxpayersByFilingStatusQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetTaxpayersByFilingStatusQuery> context)
        {
            try
            {
                var query = context.Message;
                var taxpayers = await _dbContext.Taxpayers
                    .Include(t => t.Addresses)
                    .Where(t => t.FilingStatus == query.FilingStatus && t.IsActive)
                    .OrderBy(t => t.LastName)
                    .ThenBy(t => t.FirstName)
                    .ToListAsync();

                var result = new QueryResult<IEnumerable<Taxpayer>>
                {
                    Data = taxpayers
                };

                await context.RespondAsync(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetTaxpayersByFilingStatusQuery for FilingStatus: {FilingStatus}", context.Message.FilingStatus);
                throw;
            }
        }
    }
}
