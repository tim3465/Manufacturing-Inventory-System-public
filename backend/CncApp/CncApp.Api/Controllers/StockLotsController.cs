using CncApp.Application.Dtos.StockLots;
using CncApp.Application.Services.StockLots;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for managing stock lots.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StockLotsController : ControllerBase
{
    private readonly StockLotService _stockLotService;

    public StockLotsController(StockLotService stockLotService)
    {
        _stockLotService = stockLotService;
    }

    // Conventions:
    // - All deletes are soft deletes via PATCH /{id}/inactivate.
    // - GET /all endpoints are Admin only and include inactive records.
    // - Most resources allow anonymous read access; Users requires authentication.

    /// <summary>
    /// Gets all active stock lots.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of all active stock lots.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<StockLotDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StockLotDto>>> ListAsync(CancellationToken ct = default)
    {
        var stockLots = await _stockLotService.ListActiveAsync(ct);
        return Ok(stockLots);
    }

    /// <summary>
    /// Gets a stock lot by ID.
    /// </summary>
    /// <param name="id">The stock lot ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The stock lot if found, otherwise 404.</returns>
    [HttpGet("{id:int}", Name = "GetStockLot")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(StockLotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StockLotDto>> GetAsync(int id, CancellationToken ct = default)
    {
        var stockLot = await _stockLotService.GetAsync(id, ct);
        if (stockLot == null)
        {
            return NotFound();
        }

        return Ok(stockLot);
    }

    /// <summary>
    /// Creates a new stock lot.
    /// </summary>
    /// <param name="dto">The stock lot creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created stock lot ID with Location header.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateAsync(
        [FromBody] CreateStockLotRequestDto dto,
        CancellationToken ct = default)
    {
        var id = await _stockLotService.CreateAsync(dto, ct);
        return CreatedAtRoute(routeName: "GetStockLot", routeValues: new { id }, value: new { id });
    }

    /// <summary>
    /// Updates a stock lot (metadata only - no quantity changes).
    /// </summary>
    /// <param name="id">The stock lot ID.</param>
    /// <param name="dto">The stock lot update request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>204 NoContent if successful, otherwise 404.</returns>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateStockLotRequestDto dto,
        CancellationToken ct = default)
    {
        var result = await _stockLotService.UpdateAsync(id, dto, ct);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Inactivates (soft deletes) a stock lot by ID.
    /// </summary>
    /// <param name="id">The stock lot ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>204 NoContent if successful, otherwise 404.</returns>
    [HttpPatch("{id:int}/inactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> InactivateAsync(int id, CancellationToken ct = default)
    {
        var result = await _stockLotService.InactivateAsync(id, null, ct);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}

