using Infrastructure.Commands;

namespace Application.Commands.Taxpayer
{
    public class UpdateTaxpayerAddressCommand : ICommand
    {
        public Guid TaxpayerId { get; set; }
        public Guid AddressId { get; set; }
        public AddressDto Address { get; set; } = null!;
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
