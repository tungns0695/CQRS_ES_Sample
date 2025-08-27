using Application.Queries;
using Application.ReadModels;
using Infrastructure.Queries;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.QueryHandlers
{
    public class GetTaxpayersByEmploymentStatusQueryHandler : IConsumer<GetTaxpayersByEmploymentStatusQuery>
    {
        private readonly TaxSystemDbContext _dbContext;
        private readonly ILogger<GetTaxpayersByEmploymentStatusQueryHandler> _logger;

        public GetTaxpayersByEmploymentStatusQueryHandler(TaxSystemDbContext dbContext, ILogger<GetTaxpayersByEmploymentStatusQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetTaxpayersByEmploymentStatusQuery> context)
        {
            try
            {
                var query = context.Message;
                var taxpayers = await _dbContext.Taxpayers
                    .Include(t => t.Addresses)
                    .Where(t => t.EmploymentStatus == query.EmploymentStatus && t.IsActive)
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
                _logger.LogError(ex, "Error occurred while handling GetTaxpayersByEmploymentStatusQuery for EmploymentStatus: {EmploymentStatus}", context.Message.EmploymentStatus);
                throw;
            }
        }
    }
}
