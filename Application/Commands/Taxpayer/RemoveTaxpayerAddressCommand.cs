using Infrastructure.Commands;

namespace Application.Commands.Taxpayer
{
    public class RemoveTaxpayerAddressCommand : ICommand
    {
        public Guid TaxpayerId { get; set; }
        public Guid AddressId { get; set; }
        public string RemovedBy { get; set; } = string.Empty;
    }
}
