using Microsoft.AspNetCore.Mvc;
using Application.ReadModels;
using Application.Commands.Taxpayer;
using Application.Queries;
using Infrastructure.Commands;
using MassTransit;
using Application;
using Infrastructure.Queries;

namespace TaxSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaxpayerController : ControllerBase
    {
        private readonly IRequestClient<CreateTaxpayerCommand> _createTaxpayerClient;
        private readonly IRequestClient<UpdateTaxpayerCommand> _updateTaxpayerClient;
        private readonly IRequestClient<DeactivateTaxpayerCommand> _deactivateTaxpayerClient;
        private readonly IRequestClient<UpdateTaxpayerEmploymentCommand> _updateEmploymentClient;
        private readonly IRequestClient<UpdateTaxpayerTaxFilingStatusCommand> _updateTaxFilingClient;
        
        // Query clients
        private readonly IRequestClient<GetTaxpayersQuery> _getTaxpayersClient;
        private readonly IRequestClient<GetTaxpayerQuery> _getTaxpayerClient;
        private readonly IRequestClient<GetTaxpayersByFilingStatusQuery> _getTaxpayersByFilingStatusClient;
        private readonly IRequestClient<GetTaxpayersByEmploymentStatusQuery> _getTaxpayersByEmploymentStatusClient;
        private readonly IRequestClient<GetTaxpayersWithOutstandingBalanceQuery> _getTaxpayersWithOutstandingBalanceClient;
        private readonly IRequestClient<GetTaxpayersEligibleForRefundQuery> _getTaxpayersEligibleForRefundClient;

        public TaxpayerController(
            IRequestClient<CreateTaxpayerCommand> createTaxpayerClient,
            IRequestClient<UpdateTaxpayerCommand> updateTaxpayerClient,
            IRequestClient<DeactivateTaxpayerCommand> deactivateTaxpayerClient,
            IRequestClient<UpdateTaxpayerEmploymentCommand> updateEmploymentClient,
            IRequestClient<UpdateTaxpayerTaxFilingStatusCommand> updateTaxFilingClient,
            IRequestClient<GetTaxpayersQuery> getTaxpayersClient,
            IRequestClient<GetTaxpayerQuery> getTaxpayerClient,
            IRequestClient<GetTaxpayersByFilingStatusQuery> getTaxpayersByFilingStatusClient,
            IRequestClient<GetTaxpayersByEmploymentStatusQuery> getTaxpayersByEmploymentStatusClient,
            IRequestClient<GetTaxpayersWithOutstandingBalanceQuery> getTaxpayersWithOutstandingBalanceClient,
            IRequestClient<GetTaxpayersEligibleForRefundQuery> getTaxpayersEligibleForRefundClient)
        {
            _createTaxpayerClient = createTaxpayerClient;
            _updateTaxpayerClient = updateTaxpayerClient;
            _deactivateTaxpayerClient = deactivateTaxpayerClient;
            _updateEmploymentClient = updateEmploymentClient;
            _updateTaxFilingClient = updateTaxFilingClient;
            _getTaxpayersClient = getTaxpayersClient;
            _getTaxpayerClient = getTaxpayerClient;
            _getTaxpayersByFilingStatusClient = getTaxpayersByFilingStatusClient;
            _getTaxpayersByEmploymentStatusClient = getTaxpayersByEmploymentStatusClient;
            _getTaxpayersWithOutstandingBalanceClient = getTaxpayersWithOutstandingBalanceClient;
            _getTaxpayersEligibleForRefundClient = getTaxpayersEligibleForRefundClient;
        }

        #region Read Operations

        /// <summary>
        /// Get all taxpayers with optional filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedTaxpayersResult>> GetTaxpayers(
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var query = new GetTaxpayersQuery
                {
                    SearchTerm = searchTerm,
                    IsActive = isActive,
                    Page = page,
                    PageSize = pageSize
                };

                var response = await _getTaxpayersClient.GetResponse<PaginatedTaxpayersResult>(query);
                var result = response.Message;

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving taxpayers: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a specific taxpayer by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Taxpayer>> GetTaxpayer(Guid id)
        {
            try
            {
                var query = new GetTaxpayerQuery
                {
                    TaxpayerId = id
                };

                var response = await _getTaxpayerClient.GetResponse<QueryResult<Taxpayer>>(query);
                var result = response.Message;

                if (result.Data == null)
                {
                    return NotFound($"Taxpayer with ID {id} not found.");
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the taxpayer: {ex.Message}");
            }
        }

        /// <summary>
        /// Get taxpayers by tax filing status
        /// </summary>
        [HttpGet("by-filing-status/{filingStatus}")]
        public async Task<ActionResult<IEnumerable<Taxpayer>>> GetTaxpayersByFilingStatus(string filingStatus)
        {
            try
            {
                var query = new GetTaxpayersByFilingStatusQuery
                {
                    FilingStatus = filingStatus
                };

                var response = await _getTaxpayersByFilingStatusClient.GetResponse<QueryResult<IEnumerable<Taxpayer>>>(query);
                var result = response.Message;

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving taxpayers by filing status: {ex.Message}");
            }
        }

        /// <summary>
        /// Get taxpayers by employment status
        /// </summary>
        [HttpGet("by-employment-status/{employmentStatus}")]
        public async Task<ActionResult<IEnumerable<Taxpayer>>> GetTaxpayersByEmploymentStatus(string employmentStatus)
        {
            try
            {
                var query = new GetTaxpayersByEmploymentStatusQuery
                {
                    EmploymentStatus = employmentStatus
                };

                var response = await _getTaxpayersByEmploymentStatusClient.GetResponse<QueryResult<IEnumerable<Taxpayer>>>(query);
                var result = response.Message;

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving taxpayers by employment status: {ex.Message}");
            }
        }

        /// <summary>
        /// Get taxpayers with outstanding tax balance
        /// </summary>
        [HttpGet("with-outstanding-balance")]
        public async Task<ActionResult<IEnumerable<Taxpayer>>> GetTaxpayersWithOutstandingBalance()
        {
            try
            {
                var query = new GetTaxpayersWithOutstandingBalanceQuery();

                var response = await _getTaxpayersWithOutstandingBalanceClient.GetResponse<QueryResult<IEnumerable<Taxpayer>>>(query);
                var result = response.Message;

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving taxpayers with outstanding balance: {ex.Message}");
            }
        }

        /// <summary>
        /// Get taxpayers eligible for tax refund
        /// </summary>
        [HttpGet("eligible-for-refund")]
        public async Task<ActionResult<IEnumerable<Taxpayer>>> GetTaxpayersEligibleForRefund()
        {
            try
            {
                var query = new GetTaxpayersEligibleForRefundQuery();

                var response = await _getTaxpayersEligibleForRefundClient.GetResponse<QueryResult<IEnumerable<Taxpayer>>>(query);
                var result = response.Message;

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving taxpayers eligible for refund: {ex.Message}");
            }
        }

        #endregion

        #region Write Operations

        /// <summary>
        /// Create a new taxpayer
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateTaxpayer([FromBody] CreateTaxpayerCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _createTaxpayerClient.GetResponse<CommandResult<Guid>>(command);
                var result = response.Message;

                return CreatedAtAction(nameof(GetTaxpayer), new { id = result.Data }, result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the taxpayer: {ex.Message}");
            }
        }

        /// <summary>
        /// Update an existing taxpayer
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateTaxpayer(Guid id, [FromBody] UpdateTaxpayerCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                command.TaxpayerId = id;
                var response = await _updateTaxpayerClient.GetResponse<CommandResult<bool>>(command);
                var result = response.Message;

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the taxpayer: {ex.Message}");
            }
        }

        /// <summary>
        /// Deactivate a taxpayer
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeactivateTaxpayer(Guid id, [FromBody] DeactivateTaxpayerCommand command)
        {
            try
            {
                command.TaxpayerId = id;
                var response = await _deactivateTaxpayerClient.GetResponse<CommandResult<bool>>(command);
                var result = response.Message;

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deactivating the taxpayer: {ex.Message}");
            }
        }



        /// <summary>
        /// Update a taxpayer's employment information
        /// </summary>
        [HttpPut("{id:guid}/employment")]
        public async Task<ActionResult> UpdateTaxpayerEmployment(Guid id, [FromBody] UpdateTaxpayerEmploymentCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                command.TaxpayerId = id;
                var response = await _updateEmploymentClient.GetResponse<CommandResult<bool>>(command);
                var result = response.Message;

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating employment information: {ex.Message}");
            }
        }

        /// <summary>
        /// Update a taxpayer's tax filing status
        /// </summary>
        [HttpPut("{id:guid}/tax-filing-status")]
        public async Task<ActionResult> UpdateTaxpayerTaxFilingStatus(Guid id, [FromBody] UpdateTaxpayerTaxFilingStatusCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                command.TaxpayerId = id;
                var response = await _updateTaxFilingClient.GetResponse<CommandResult<bool>>(command);
                var result = response.Message;

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating tax filing status: {ex.Message}");
            }
        }

        #endregion
    }
}
