using Infrastructure.Events;

namespace Application.Domains.Events
{
    public class TaxpayerCreatedEvent : IEvent
    {
        public required string Id { get; set; }
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public DateTimeOffset OccuredOn { get; set; }
        public long Position { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string SocialSecurityNumber { get; set; } = string.Empty;
        public string TaxIdentificationNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<Address> Addresses { get; set; } = new List<Address>();
        public string CreatedBy { get; set; } = string.Empty;
    }
}
