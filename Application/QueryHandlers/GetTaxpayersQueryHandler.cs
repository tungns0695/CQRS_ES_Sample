using Application.Queries;
using Application.ReadModels;
using Infrastructure.Queries;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.QueryHandlers
{
    public class GetTaxpayersQueryHandler : IConsumer<GetTaxpayersQuery>
    {
        private readonly TaxSystemDbContext _dbContext;
        private readonly ILogger<GetTaxpayersQueryHandler> _logger;

        public GetTaxpayersQueryHandler(TaxSystemDbContext dbContext, ILogger<GetTaxpayersQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetTaxpayersQuery> context)
        {
            try
            {
                var query = context.Message;
                var dbQuery = _dbContext.Taxpayers
                    .Include(t => t.Addresses)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(query.SearchTerm))
                {
                    dbQuery = dbQuery.Where(t => 
                        t.FirstName.Contains(query.SearchTerm) || 
                        t.LastName.Contains(query.SearchTerm) || 
                        t.Email.Contains(query.SearchTerm) ||
                        t.SocialSecurityNumber.Contains(query.SearchTerm));
                }

                if (query.IsActive.HasValue)
                {
                    dbQuery = dbQuery.Where(t => t.IsActive == query.IsActive.Value);
                }

                // Get total count
                var totalCount = await dbQuery.CountAsync();

                // Apply pagination
                var taxpayers = await dbQuery
                    .OrderBy(t => t.LastName)
                    .ThenBy(t => t.FirstName)
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

                var result = new PaginatedTaxpayersResult
                {
                    Data = taxpayers,
                    TotalCount = totalCount,
                    Page = query.Page,
                    PageSize = query.PageSize,
                    TotalPages = totalPages
                };

                await context.RespondAsync(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetTaxpayersQuery");
                throw;
            }
        }
    }
}
