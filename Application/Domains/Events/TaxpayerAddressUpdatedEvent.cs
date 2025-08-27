using Infrastructure.Events;

namespace Application.Domains.Events
{
    public class TaxpayerAddressUpdatedEvent : IEvent
    {
        public required string Id { get; set; }
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public DateTimeOffset OccuredOn { get; set; }
        public long Position { get; set; }
        public Guid AddressId { get; set; }
        public Address Address { get; set; } = null!;
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
