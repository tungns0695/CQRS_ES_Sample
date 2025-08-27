using Infrastructure.Repository;

namespace Application.ReadModels
{
    public class Taxpayer : BaseEntity
    {
        // Personal Identification
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string SocialSecurityNumber { get; set; } = string.Empty;
        public string TaxIdentificationNumber { get; set; } = string.Empty;

        // Contact Information
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<TaxpayerAddress> Addresses { get; set; } = new List<TaxpayerAddress>();

        // Tax Information
        public string FilingStatus { get; set; } = string.Empty;
        public decimal AnnualIncome { get; set; }
        public string EmploymentStatus { get; set; } = string.Empty;
        public string EmployerName { get; set; } = string.Empty;
        public string EmployerIdentificationNumber { get; set; } = string.Empty;

        // Status and Metadata
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string LastModifiedBy { get; set; } = string.Empty;

        // Tax Filing Information
        public int TaxYear { get; set; }
        public bool HasFiledTaxes { get; set; } = false;
        public DateTime? TaxFilingDate { get; set; }
        public decimal TaxLiability { get; set; }
        public decimal TaxPaid { get; set; }
        public decimal TaxRefund { get; set; }

        // Event Sourcing Tracking
        public int Version { get; set; }
        public string? LastEventId { get; set; }
        public long? LastEventPosition { get; set; }

        // Computed Properties
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string FullNameWithMiddle => $"{FirstName} {MiddleName} {LastName}".Trim();
        public int Age => DateTime.Today.Year - DateOfBirth.Year - (DateTime.Today < DateOfBirth.AddYears(DateTime.Today.Year - DateOfBirth.Year) ? 1 : 0);
        public TaxpayerAddress? PrimaryAddress => Addresses.FirstOrDefault(a => a.IsPrimary);
        public decimal TaxBalance => TaxLiability - TaxPaid;
        public bool IsTaxRefundEligible => TaxPaid > TaxLiability;
    }
}
