using Infrastructure.Events;

namespace Application.Domains.Events
{
    public class TaxpayerAddressRemovedEvent : IEvent
    {
        public required string Id { get; set; }
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public long Position { get; set; }
        public DateTimeOffset OccuredOn { get; set; }
        
        public Guid AddressId { get; set; }
        public string RemovedBy { get; set; } = string.Empty;
    }
}
