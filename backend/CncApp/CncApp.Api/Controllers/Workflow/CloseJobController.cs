using CncApp.Application.Dtos.CloseJob;
using CncApp.Application.Services.Users;
using CncApp.Application.Services.Workflows.CloseJob;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers.Workflow;

/// <summary>
/// Workflow controller for closing a job and its associated shift atomically.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CloseJobController : ControllerBase
{
    private readonly CloseJobService _closeJobService;
    private readonly UserService _userService;

    public CloseJobController(CloseJobService closeJobService, UserService userService)
    {
        _closeJobService = closeJobService;
        _userService = userService;
    }

    /// <summary>
    /// Closes a shift and its parent job atomically.
    /// </summary>
    /// <param name="dto">The close job request containing shift and job identifiers plus shift data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The closed job and shift details.</returns>
    [HttpPost("close")]
    [Authorize(Roles = "Machinist,Admin")]
    [ProducesResponseType(typeof(CloseJobResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CloseJobResponseDto>> CloseJobAsync(
        [FromBody] CloseJobRequestDto dto, CancellationToken ct = default)
    {
        var operator_ = await _userService.GetCurrentUserAsync(ct);
        var result = await _closeJobService.CloseJobAsync(dto, operator_.Id, ct);
        return Ok(result);
    }
}
