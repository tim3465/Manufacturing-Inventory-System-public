using CncApp.Application.Dtos.Orders;
using CncApp.Application.Services.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for managing orders.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="dto">The order creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created order ID with Location header.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateAsync(
        [FromBody] CreateOrderRequestDto dto,
        CancellationToken ct = default)
    {
        var id = await _orderService.CreateAsync(dto, ct);
        return CreatedAtRoute(routeName: "GetOrder", routeValues: new { id }, value: new { id });
    }

    /// <summary>
    /// Updates an order by ID (metadata only).
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="dto">The order update request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated order DTO if found, otherwise 404.</returns>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> UpdateAsync(
        int id,
        [FromBody] UpdateOrderRequestDto dto,
        CancellationToken ct = default)
    {
        var order = await _orderService.UpdateAsync(id, dto, ct);
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    /// <summary>
    /// Inactivates (soft deletes) an order by ID.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>204 NoContent if successful, otherwise 404.</returns>
    [HttpPatch("{id:int}/inactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> InactivateAsync(int id, CancellationToken ct = default)
    {
        var result = await _orderService.InactivateAsync(id, null, ct);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Gets all active orders.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of all active orders.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderDto>>> ListAsync(CancellationToken ct = default)
    {
        var orders = await _orderService.ListActiveAsync(ct);
        return Ok(orders);
    }

    /// <summary>
    /// Gets all orders (including inactive).
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of all orders.</returns>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderDto>>> ListAllAsync(CancellationToken ct = default)
    {
        var orders = await _orderService.ListAllAsync(ct);
        return Ok(orders);
    }

    /// <summary>
    /// Gets an order by ID.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The order if found, otherwise 404.</returns>
    [HttpGet("{id:int}", Name = "GetOrder")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetAsync(int id, CancellationToken ct = default)
    {
        var order = await _orderService.GetAsync(id, ct);
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }
}

