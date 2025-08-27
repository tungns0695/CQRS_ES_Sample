using Application.ReadModels;
using Infrastructure.Queries;

namespace Application.Queries
{
    public class PaginatedTaxpayersResult : IQueryResult<IEnumerable<Taxpayer>>
    {
        public IEnumerable<Taxpayer>? Data { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
