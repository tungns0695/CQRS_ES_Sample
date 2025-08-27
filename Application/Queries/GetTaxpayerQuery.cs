using Infrastructure.Queries;

namespace Application.Queries
{
    public class GetTaxpayerQuery : IQuery
    {
        public Guid TaxpayerId { get; set; }
    }
}
