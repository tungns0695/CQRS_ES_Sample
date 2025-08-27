using Infrastructure.Repository;

namespace Application.ReadModels
{
    public class TaxpayerAddress : BaseEntity
    {
        // Address Details
        public string StreetAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string AddressType { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;

        // Metadata
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        // Foreign Key
        public Guid TaxpayerId { get; set; }

        // Event Sourcing Tracking
        public int Version { get; set; }
        public string? LastEventId { get; set; }
        public long? LastEventPosition { get; set; }

        // Computed Properties
        public string FullAddress => $"{StreetAddress}, {City}, {State} {ZipCode}, {Country}".Trim();
        public string CityStateZip => $"{City}, {State} {ZipCode}".Trim();
        public string StateZip => $"{State} {ZipCode}".Trim();
        public bool IsDomestic => Country.Equals("USA", StringComparison.OrdinalIgnoreCase) || 
                                  Country.Equals("United States", StringComparison.OrdinalIgnoreCase) ||
                                  Country.Equals("US", StringComparison.OrdinalIgnoreCase);
    }
}
