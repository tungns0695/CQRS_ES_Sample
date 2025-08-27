using Infrastructure.Queries;

namespace Application.Queries
{
    public class GetTaxpayersByFilingStatusQuery : IQuery
    {
        public string FilingStatus { get; set; } = string.Empty;
    }
}
