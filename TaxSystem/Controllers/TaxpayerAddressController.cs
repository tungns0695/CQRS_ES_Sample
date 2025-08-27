using Microsoft.AspNetCore.Mvc;
using Application.Commands.Taxpayer;
using Infrastructure.Commands;
using MassTransit;

namespace TaxSystem.Controllers
{
    [ApiController]
    [Route("api/taxpayers/{taxpayerId:guid}/addresses")]
    public class TaxpayerAddressController : ControllerBase
    {
        private readonly IRequestClient<AddTaxpayerAddressCommand> _addAddressClient;
        private readonly IRequestClient<UpdateTaxpayerAddressCommand> _updateAddressClient;
        private readonly IRequestClient<RemoveTaxpayerAddressCommand> _removeAddressClient;

        public TaxpayerAddressController(
            IRequestClient<AddTaxpayerAddressCommand> addAddressClient,
            IRequestClient<UpdateTaxpayerAddressCommand> updateAddressClient,
            IRequestClient<RemoveTaxpayerAddressCommand> removeAddressClient)
        {
            _addAddressClient = addAddressClient;
            _updateAddressClient = updateAddressClient;
            _removeAddressClient = removeAddressClient;
        }

        /// <summary>
        /// Add an address to a taxpayer
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> AddTaxpayerAddress(Guid taxpayerId, [FromBody] AddTaxpayerAddressCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                command.TaxpayerId = taxpayerId;
                var response = await _addAddressClient.GetResponse<CommandResult<bool>>(command);
                var result = response.Message;

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the address: {ex.Message}");
            }
        }

        /// <summary>
        /// Update a taxpayer's address
        /// </summary>
        [HttpPut("{addressId:guid}")]
        public async Task<ActionResult> UpdateTaxpayerAddress(Guid taxpayerId, Guid addressId, [FromBody] UpdateTaxpayerAddressCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                command.TaxpayerId = taxpayerId;
                command.AddressId = addressId;
                var response = await _updateAddressClient.GetResponse<CommandResult<bool>>(command);
                var result = response.Message;

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the address: {ex.Message}");
            }
        }

        /// <summary>
        /// Remove a taxpayer's address
        /// </summary>
        [HttpDelete("{addressId:guid}")]
        public async Task<ActionResult> RemoveTaxpayerAddress(Guid taxpayerId, Guid addressId, [FromBody] RemoveTaxpayerAddressCommand command)
        {
            try
            {
                command.TaxpayerId = taxpayerId;
                command.AddressId = addressId;
                var response = await _removeAddressClient.GetResponse<CommandResult<bool>>(command);
                var result = response.Message;

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while removing the address: {ex.Message}");
            }
        }
    }
}
