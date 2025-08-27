using Infrastructure.Commands;

namespace Application.Commands.Taxpayer
{
    public class DeactivateTaxpayerCommand : ICommand
    {
        public Guid TaxpayerId { get; set; }
        public string DeactivatedBy { get; set; } = string.Empty;
    }
}
