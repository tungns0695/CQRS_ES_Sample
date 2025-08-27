using Infrastructure.Commands;

namespace Application.Commands.Taxpayer
{
    public class AddTaxpayerAddressCommand : ICommand
    {
        public Guid TaxpayerId { get; set; }
        public AddressDto Address { get; set; } = null!;
        public string AddedBy { get; set; } = string.Empty;
    }
}
