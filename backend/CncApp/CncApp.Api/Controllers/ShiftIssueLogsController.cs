using CncApp.Application.Dtos.ShiftIssueLogs;
using CncApp.Application.Services.ShiftIssueLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for creating shift issue logs (scrap/downtime events).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ShiftIssueLogsController : ControllerBase
{
    private readonly ShiftIssueLogService _shiftIssueLogService;

    public ShiftIssueLogsController(ShiftIssueLogService shiftIssueLogService)
    {
        _shiftIssueLogService = shiftIssueLogService;
    }

    /// <summary>
    /// Creates a new shift issue log entry for scrap or downtime.
    /// </summary>
    /// <param name="dto">The shift issue log creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created shift issue log ID.</returns>
    [HttpPost]
    [Authorize(Roles = "Machinist,Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateAsync(
        [FromBody] CreateShiftIssueLogRequestDto dto,
        CancellationToken ct = default)
    {
        var id = await _shiftIssueLogService.CreateAsync(dto, ct);
        return StatusCode(StatusCodes.Status201Created, new { id });
    }
}
