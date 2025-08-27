using Infrastructure.Events;

namespace Application.Domains.Events
{
    public class TaxpayerEmploymentUpdatedEvent : IEvent
    {
        public required string Id { get; set; }
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public DateTimeOffset OccuredOn { get; set; }
        public long Position { get; set; }
        public string EmploymentStatus { get; set; } = string.Empty;
        public string EmployerName { get; set; } = string.Empty;
        public string EmployerIdentificationNumber { get; set; } = string.Empty;
        public decimal AnnualIncome { get; set; }
        public string LastModifiedBy { get; set; } = string.Empty;
    }
}
