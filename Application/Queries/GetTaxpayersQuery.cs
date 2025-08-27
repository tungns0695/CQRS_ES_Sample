using Infrastructure.Queries;

namespace Application.Queries
{
    public class GetTaxpayersQuery : IQuery
    {
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
