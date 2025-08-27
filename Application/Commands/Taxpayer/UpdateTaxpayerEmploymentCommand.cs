using Infrastructure.Commands;
using System.ComponentModel.DataAnnotations;

namespace Application.Commands.Taxpayer
{
    public class UpdateTaxpayerEmploymentCommand : ICommand
    {
        public Guid TaxpayerId { get; set; }

        [Required]
        [StringLength(50)]
        public string EmploymentStatus { get; set; } = string.Empty;

        [StringLength(255)]
        public string EmployerName { get; set; } = string.Empty;

        [StringLength(20)]
        public string EmployerIdentificationNumber { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal AnnualIncome { get; set; }

        public string LastModifiedBy { get; set; } = string.Empty;
    }
}
