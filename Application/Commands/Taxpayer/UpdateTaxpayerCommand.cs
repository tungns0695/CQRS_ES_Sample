using Infrastructure.Commands;
using System.ComponentModel.DataAnnotations;

namespace Application.Commands.Taxpayer
{
    public class UpdateTaxpayerCommand : ICommand
    {
        public Guid TaxpayerId { get; set; }

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

        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public List<AddressDto> Addresses { get; set; } = new List<AddressDto>();

        public string LastModifiedBy { get; set; } = string.Empty;
    }
}
