using Infrastructure.Commands;
using System.ComponentModel.DataAnnotations;

namespace Application.Commands.Taxpayer
{
    public class UpdateTaxpayerTaxFilingStatusCommand : ICommand
    {
        public Guid TaxpayerId { get; set; }

        [Required]
        [StringLength(50)]
        public string FilingStatus { get; set; } = string.Empty;

        [Range(1900, 2100)]
        public int TaxYear { get; set; }

        public bool HasFiledTaxes { get; set; }

        public DateTime? TaxFilingDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxLiability { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxPaid { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxRefund { get; set; }

        public string LastModifiedBy { get; set; } = string.Empty;
    }
}
