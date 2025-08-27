using Infrastructure.Commands;
using System.ComponentModel.DataAnnotations;

namespace Application.Commands.Taxpayer
{
    public class CreateTaxpayerCommand : ICommand
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string MiddleName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(11)]
        public string SocialSecurityNumber { get; set; } = string.Empty;

        [StringLength(20)]
        public string TaxIdentificationNumber { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public List<AddressDto> Addresses { get; set; } = new List<AddressDto>();

        public string CreatedBy { get; set; } = string.Empty;
    }
}
