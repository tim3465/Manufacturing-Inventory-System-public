using CncApp.Application.Dtos.StockLotAdjustments;
using CncApp.Application.Services.StockLotAdjustments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for managing stock lot adjustments.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StockLotAdjustmentsController : ControllerBase
{
    private readonly StockLotAdjustmentService _stockLotAdjustmentService;

    public StockLotAdjustmentsController(StockLotAdjustmentService stockLotAdjustmentService)
    {
        _stockLotAdjustmentService = stockLotAdjustmentService;
    }

    // Conventions:
    // - All deletes are soft deletes via PATCH /{id}/inactivate.
    // - GET /all endpoints are Admin only and include inactive records.
    // - Most resources allow anonymous read access; Users requires authentication.

    /// <summary>
    /// Gets a stock lot adjustment by ID.
    /// </summary>
    /// <param name="id">The stock lot adjustment ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The stock lot adjustment if found, otherwise 404.</returns>
    [HttpGet("{id:int}", Name = "GetStockLotAdjustment")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(StockLotAdjustmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StockLotAdjustmentDto>> GetAsync(int id, CancellationToken ct = default)
    {
        var stockLotAdjustment = await _stockLotAdjustmentService.GetAsync(id, ct);
        if (stockLotAdjustment == null)
        {
            return NotFound();
        }

        return Ok(stockLotAdjustment);
    }

    /// <summary>
    /// Gets all stock lot adjustments for a specific stock lot.
    /// </summary>
    /// <param name="stockLotId">The stock lot ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of active stock lot adjustments for the stock lot.</returns>
    [HttpGet("by-stocklot/{stockLotId:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<StockLotAdjustmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StockLotAdjustmentDto>>> ListByStockLotAsync(int stockLotId, CancellationToken ct = default)
    {
        var stockLotAdjustments = await _stockLotAdjustmentService.ListByStockLotAsync(stockLotId, ct);
        return Ok(stockLotAdjustments);
    }

    /// <summary>
    /// Gets all stock lot adjustments.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of all stock lot adjustments.</returns>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<StockLotAdjustmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StockLotAdjustmentDto>>> ListAllAsync(CancellationToken ct = default)
    {
        var stockLotAdjustments = await _stockLotAdjustmentService.ListAllAsync(ct);
        return Ok(stockLotAdjustments);
    }

    /// <summary>
    /// Creates a new stock lot adjustment.
    /// </summary>
    /// <param name="dto">The stock lot adjustment creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created stock lot adjustment ID with Location header.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Shipping")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateAsync(
        [FromBody] CreateStockLotAdjustmentRequestDto dto,
        CancellationToken ct = default)
    {
        var id = await _stockLotAdjustmentService.CreateAsync(dto, ct);
        return CreatedAtRoute(routeName: "GetStockLotAdjustment", routeValues: new { id }, value: new { id });
    }

    /// <summary>
    /// Updates the notes for a stock lot adjustment (metadata-only update).
    /// </summary>
    /// <param name="id">The stock lot adjustment ID.</param>
    /// <param name="dto">The notes update request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated stock lot adjustment if found, otherwise 404.</returns>
    [HttpPatch("{id:int}/notes")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(StockLotAdjustmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StockLotAdjustmentDto>> UpdateNotesAsync(
        int id,
        [FromBody] UpdateStockLotAdjustmentNotesRequestDto dto,
        CancellationToken ct = default)
    {
        var stockLotAdjustment = await _stockLotAdjustmentService.UpdateNotesAsync(id, dto, ct);
        if (stockLotAdjustment == null)
        {
            return NotFound();
        }

        return Ok(stockLotAdjustment);
    }

    /// <summary>
    /// Inactivates (soft deletes) a stock lot adjustment by ID.
    /// </summary>
    /// <param name="id">The stock lot adjustment ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>204 NoContent if successful, otherwise 404.</returns>
    [HttpPatch("{id:int}/inactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> InactivateAsync(int id, CancellationToken ct = default)
    {
        var result = await _stockLotAdjustmentService.InactivateAsync(id, null, ct);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}

