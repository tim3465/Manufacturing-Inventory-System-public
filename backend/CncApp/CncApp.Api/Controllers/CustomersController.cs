using CncApp.Application.Dtos.Customers;
using CncApp.Application.Services.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for managing customers.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    // Conventions:
    // - All deletes are soft deletes via PATCH /{id}/inactivate.
    // - GET /all endpoints are Admin only and include inactive records.
    // - Most resources allow anonymous read access.

    /// <summary>
    /// Gets all active customers.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CustomerDto>>> ListAsync(CancellationToken ct = default)
    {
        var customers = await _customerService.ListActiveAsync(ct);
        return Ok(customers);
    }

    /// <summary>
    /// Gets a customer by ID.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetCustomer")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetAsync(int id, CancellationToken ct = default)
    {
        var customer = await _customerService.GetAsync(id, ct);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    /// <summary>
    /// Gets all customers (including inactive). Admin only.
    /// </summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CustomerDto>>> ListAllAsync(CancellationToken ct = default)
    {
        var customers = await _customerService.ListAllAsync(ct);
        return Ok(customers);
    }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Supervisor,Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateAsync(
        [FromBody] CreateCustomerRequestDto dto,
        CancellationToken ct = default)
    {
        var id = await _customerService.CreateAsync(dto, ct);
        return CreatedAtRoute(routeName: "GetCustomer", routeValues: new { id }, value: new { id });
    }

    /// <summary>
    /// Updates a customer by ID.
    /// </summary>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Supervisor,Admin")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> UpdateAsync(
        int id,
        [FromBody] UpdateCustomerRequestDto dto,
        CancellationToken ct = default)
    {
        var customer = await _customerService.UpdateAsync(id, dto, ct);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    /// <summary>
    /// Inactivates (soft deletes) a customer by ID. Admin only.
    /// </summary>
    [HttpPatch("{id:int}/inactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> InactivateAsync(int id, CancellationToken ct = default)
    {
        var result = await _customerService.InactivateAsync(id, null, ct);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
