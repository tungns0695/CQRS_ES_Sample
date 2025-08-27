using Infrastructure.Aggregate;
using Application.Domains.Events;
using Infrastructure.Events;

namespace Application.Domains
{
    public class TaxpayerAggregate : AggregateRoot
    {
        // Personal Identification
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string MiddleName { get; private set; } = string.Empty;
        public DateTime DateOfBirth { get; private set; }
        public string SocialSecurityNumber { get; private set; } = string.Empty;
        public string TaxIdentificationNumber { get; private set; } = string.Empty;

        // Contact Information
        public string Email { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public List<Address> Addresses { get; private set; } = new List<Address>();

        // Tax Information
        public string FilingStatus { get; private set; } = string.Empty;
        public decimal AnnualIncome { get; private set; }
        public string EmploymentStatus { get; private set; } = string.Empty;
        public string EmployerName { get; private set; } = string.Empty;
        public string EmployerIdentificationNumber { get; private set; } = string.Empty;

        // Status and Metadata
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModifiedDate { get; private set; }
        public string CreatedBy { get; private set; } = string.Empty;
        public string LastModifiedBy { get; private set; } = string.Empty;

        // Tax Filing Information
        public int TaxYear { get; private set; }
        public bool HasFiledTaxes { get; private set; } = false;
        public DateTime? TaxFilingDate { get; private set; }
        public decimal TaxLiability { get; private set; }
        public decimal TaxPaid { get; private set; }
        public decimal TaxRefund { get; private set; }

        protected override void Mutate(IEvent @event)
        {
            switch (@event)
            {
                case TaxpayerCreatedEvent taxpayerCreatedEvent:
                    When(taxpayerCreatedEvent);
                    break;
                case TaxpayerUpdatedEvent taxpayerUpdatedEvent:
                    When(taxpayerUpdatedEvent);
                    break;
                case TaxpayerEmploymentUpdatedEvent taxpayerEmploymentUpdatedEvent:
                    When(taxpayerEmploymentUpdatedEvent);
                    break;
                case TaxpayerTaxFilingStatusUpdatedEvent taxpayerTaxFilingStatusUpdatedEvent:
                    When(taxpayerTaxFilingStatusUpdatedEvent);
                    break;
                case TaxpayerDeactivatedEvent taxpayerDeactivatedEvent:
                    When(taxpayerDeactivatedEvent);
                    break;
                case TaxpayerAddressAddedEvent taxpayerAddressAddedEvent:
                    When(taxpayerAddressAddedEvent);
                    break;
                case TaxpayerAddressUpdatedEvent taxpayerAddressUpdatedEvent:
                    When(taxpayerAddressUpdatedEvent);
                    break;
                case TaxpayerAddressRemovedEvent taxpayerAddressRemovedEvent:
                    When(taxpayerAddressRemovedEvent);
                    break;
                default:
                    throw new ArgumentException($"Unknown event type: {@event.GetType().Name}", nameof(@event));
            }
        }

        // When methods for handling domain events
        private void When(TaxpayerCreatedEvent @event)
        {
            AggregateId = @event.AggregateId;
            FirstName = @event.FirstName;
            LastName = @event.LastName;
            MiddleName = @event.MiddleName;
            DateOfBirth = @event.DateOfBirth;
            SocialSecurityNumber = @event.SocialSecurityNumber;
            TaxIdentificationNumber = @event.TaxIdentificationNumber;
            Email = @event.Email;
            PhoneNumber = @event.PhoneNumber;
            Addresses = @event.Addresses;
            CreatedBy = @event.CreatedBy;
            CreatedDate = @event.OccuredOn.DateTime;
            IsActive = true;
        }

        private void When(TaxpayerUpdatedEvent @event)
        {
            FirstName = @event.FirstName;
            LastName = @event.LastName;
            MiddleName = @event.MiddleName;
            DateOfBirth = @event.DateOfBirth;
            Email = @event.Email;
            PhoneNumber = @event.PhoneNumber;
            Addresses = @event.Addresses;
            LastModifiedBy = @event.LastModifiedBy;
            LastModifiedDate = @event.OccuredOn.DateTime;
        }

        private void When(TaxpayerEmploymentUpdatedEvent @event)
        {
            EmploymentStatus = @event.EmploymentStatus;
            EmployerName = @event.EmployerName;
            EmployerIdentificationNumber = @event.EmployerIdentificationNumber;
            AnnualIncome = @event.AnnualIncome;
            LastModifiedBy = @event.LastModifiedBy;
            LastModifiedDate = @event.OccuredOn.DateTime;
        }

        private void When(TaxpayerTaxFilingStatusUpdatedEvent @event)
        {
            FilingStatus = @event.FilingStatus;
            TaxYear = @event.TaxYear;
            HasFiledTaxes = @event.HasFiledTaxes;
            TaxFilingDate = @event.TaxFilingDate;
            TaxLiability = @event.TaxLiability;
            TaxPaid = @event.TaxPaid;
            TaxRefund = @event.TaxRefund;
            LastModifiedBy = @event.LastModifiedBy;
            LastModifiedDate = @event.OccuredOn.DateTime;
        }

        private void When(TaxpayerDeactivatedEvent @event)
        {
            IsActive = false;
            LastModifiedBy = @event.DeactivatedBy;
            LastModifiedDate = @event.OccuredOn.DateTime;
        }

        private void When(TaxpayerAddressAddedEvent @event)
        {
            Addresses.Add(@event.Address);
            LastModifiedBy = @event.AddedBy;
            LastModifiedDate = @event.OccuredOn.DateTime;
        }

        private void When(TaxpayerAddressUpdatedEvent @event)
        {
            var existingAddress = Addresses.FirstOrDefault(a => a.Id == @event.AddressId);
            if (existingAddress != null)
            {
                var index = Addresses.IndexOf(existingAddress);
                Addresses[index] = @event.Address;
            }
            LastModifiedBy = @event.UpdatedBy;
            LastModifiedDate = @event.OccuredOn.DateTime;
        }

        private void When(TaxpayerAddressRemovedEvent @event)
        {
            var addressToRemove = Addresses.FirstOrDefault(a => a.Id == @event.AddressId);
            if (addressToRemove != null)
            {
                Addresses.Remove(addressToRemove);
            }
            LastModifiedBy = @event.RemovedBy;
            LastModifiedDate = @event.OccuredOn.DateTime;
        }
    }

    public class Address
    {
        public Guid Id { get; private set; }
        public string StreetAddress { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string State { get; private set; } = string.Empty;
        public string ZipCode { get; private set; } = string.Empty;
        public string Country { get; private set; } = string.Empty;
        public string AddressType { get; private set; } = string.Empty;
        public bool IsPrimary { get; private set; } = false;
        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModifiedDate { get; private set; }

        public Address(
            Guid id,
            string streetAddress,
            string city,
            string state,
            string zipCode,
            string country,
            string addressType,
            bool isPrimary = false)
        {
            Id = id;
            StreetAddress = streetAddress;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
            AddressType = addressType;
            IsPrimary = isPrimary;
            CreatedDate = DateTime.UtcNow;
        }

        public void UpdateAddress(
            string streetAddress,
            string city,
            string state,
            string zipCode,
            string country,
            string addressType,
            bool isPrimary)
        {
            StreetAddress = streetAddress;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
            AddressType = addressType;
            IsPrimary = isPrimary;
            LastModifiedDate = DateTime.UtcNow;
        }

        public void SetAsPrimary()
        {
            IsPrimary = true;
            LastModifiedDate = DateTime.UtcNow;
        }

        public void SetAsNonPrimary()
        {
            IsPrimary = false;
            LastModifiedDate = DateTime.UtcNow;
        }
    }
}
