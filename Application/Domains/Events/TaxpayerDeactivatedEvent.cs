using Infrastructure.Events;

namespace Application.Domains.Events
{
    public class TaxpayerDeactivatedEvent : IEvent
    {
        public required string Id { get; set; }
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public DateTimeOffset OccuredOn { get; set; }
        public long Position { get; set; }
        public string DeactivatedBy { get; set; } = string.Empty;
    }
}
