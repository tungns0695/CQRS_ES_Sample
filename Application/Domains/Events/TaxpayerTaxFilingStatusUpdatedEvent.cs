using Infrastructure.Events;

namespace Application.Domains.Events
{
    public class TaxpayerTaxFilingStatusUpdatedEvent : IEvent
    {
        public required string Id { get; set; }
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public DateTimeOffset OccuredOn { get; set; }
        public long Position { get; set; }
        public string FilingStatus { get; set; } = string.Empty;
        public int TaxYear { get; set; }
        public bool HasFiledTaxes { get; set; }
        public DateTime? TaxFilingDate { get; set; }
        public decimal TaxLiability { get; set; }
        public decimal TaxPaid { get; set; }
        public decimal TaxRefund { get; set; }
        public string LastModifiedBy { get; set; } = string.Empty;
    }
}
