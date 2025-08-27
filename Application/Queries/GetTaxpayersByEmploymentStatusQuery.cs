using Infrastructure.Queries;

namespace Application.Queries
{
    public class GetTaxpayersByEmploymentStatusQuery : IQuery
    {
        public string EmploymentStatus { get; set; } = string.Empty;
    }
}
