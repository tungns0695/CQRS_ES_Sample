using Infrastructure.Events;

namespace Application.Domains.Events
{
    public class TaxpayerAddressAddedEvent : IEvent
    {
        public required string Id { get; set; }
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public long Position { get; set; }
        public DateTimeOffset OccuredOn { get; set; }
        
        public Address Address { get; set; } = null!;
        public string AddedBy { get; set; } = string.Empty;
    }
}
