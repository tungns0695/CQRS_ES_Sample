using System.ComponentModel.DataAnnotations;

namespace Application.Commands.Taxpayer
{
    public class AddressDto
    {
        [Required]
        [StringLength(255)]
        public string StreetAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string ZipCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AddressType { get; set; } = string.Empty;

        public bool IsPrimary { get; set; } = false;
    }
}
