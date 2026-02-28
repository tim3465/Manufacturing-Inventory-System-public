using CncApp.Application.Dtos.ShippingReceiving;
using CncApp.Application.Services.Workflows.ShippingReceiving;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers.Workflow;

/// <summary>
/// Workflow controller for shipping and receiving operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ShippingReceivingController : ControllerBase
{
    private readonly ShippingReceivingService _shippingReceivingService;

    public ShippingReceivingController(ShippingReceivingService shippingReceivingService)
    {
        _shippingReceivingService = shippingReceivingService;
    }

    /// <summary>
    /// Receives a shipment: creates Material (if needed), StockLot, StockLotAdjustment,
    /// and updates StockLot.AmountOfBars atomically.
    /// </summary>
    /// <param name="dto">The receive shipment request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The IDs of all created entities.</returns>
    [HttpPost("receive")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ReceiveShipmentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReceiveShipmentResponseDto>> ReceiveShipmentAsync(
        [FromBody] ReceiveShipmentRequestDto dto,
        CancellationToken ct = default)
    {
        var result = await _shippingReceivingService.ReceiveShipmentAsync(dto, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
