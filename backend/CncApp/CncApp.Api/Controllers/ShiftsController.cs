using CncApp.Application.Dtos.Shifts;
using CncApp.Application.Services.Shifts;
using CncApp.Application.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly ShiftService _shiftService;
    private readonly UserService _userService;

    public ShiftsController(ShiftService shiftService, UserService userService)
    {
        _shiftService = shiftService;
        _userService = userService;
    }

    // Conventions:
    // - All deletes are soft deletes via PATCH /{id}/inactivate.
    // - GET /all endpoints are Admin only and include inactive records.
    // - Most resources allow anonymous read access; Users requires authentication.

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ShiftDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ShiftDto>>> ListAsync(CancellationToken ct = default)
    {
        var shifts = await _shiftService.ListActiveAsync(ct);
        return Ok(shifts);
    }

    [HttpGet("{id:int}", Name = "GetShift")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ShiftDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShiftDto>> GetAsync(int id, CancellationToken ct = default)
    {
        var shift = await _shiftService.GetAsync(id, ct);
        if (shift == null)
        {
            return NotFound();
        }

        return Ok(shift);
    }

    [HttpGet("production")]
    [Authorize(Roles = "Admin,Supervisor")]
    [ProducesResponseType(typeof(List<ShiftDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ShiftDto>>> ListProductionAsync(CancellationToken ct = default)
    {
        var shifts = await _shiftService.ListProductionAsync(ct);
        return Ok(shifts);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<ShiftDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ShiftDto>>> ListAllAsync(CancellationToken ct = default)
    {
        var shifts = await _shiftService.ListAllAsync(ct);
        return Ok(shifts);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateAsync([FromBody] CreateShiftRequestDto dto, CancellationToken ct = default)
    {
        var id = await _shiftService.CreateAsync(dto, ct);
        return CreatedAtRoute(routeName: "GetShift", routeValues: new { id }, value: new { id });
    }

    [HttpPatch("{id:int}/inactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> InactivateAsync(int id, CancellationToken ct = default)
    {
        var result = await _shiftService.InactivateAsync(id, null, ct);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("start")]
    [Authorize(Roles = "Machinist,Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> StartShiftAsync([FromBody] StartShiftRequestDto dto, CancellationToken ct = default)
    {
        var operator_ = await _userService.GetCurrentUserAsync(ct);
        var id = await _shiftService.StartShiftAsync(dto, operator_.Id, ct);
        return CreatedAtRoute(routeName: "GetShift", routeValues: new { id }, value: new { id });
    }

    [HttpGet("running")]
    [Authorize(Roles = "Machinist,Admin")]
    [ProducesResponseType(typeof(List<RunningShiftDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RunningShiftDto>>> ListRunningShiftsAsync(CancellationToken ct = default)
    {
        var operator_ = await _userService.GetCurrentUserAsync(ct);
        var shifts = await _shiftService.ListRunningShiftsAsync(operator_.Id, ct);
        return Ok(shifts);
    }

    [HttpGet("{id:int}/running")]
    [Authorize(Roles = "Machinist,Admin")]
    [ProducesResponseType(typeof(RunningShiftDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RunningShiftDto>> GetRunningShiftAsync(int id, CancellationToken ct = default)
    {
        var operator_ = await _userService.GetCurrentUserAsync(ct);
        var shift = await _shiftService.GetRunningShiftAsync(id, operator_.Id, ct);
        if (shift == null)
        {
            return NotFound();
        }

        return Ok(shift);
    }

    [HttpPatch("{id:int}/save")]
    [Authorize(Roles = "Machinist,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SaveShiftAsync(int id, [FromBody] UpdateShiftRequestDto dto, CancellationToken ct = default)
    {
        var operator_ = await _userService.GetCurrentUserAsync(ct);
        var result = await _shiftService.UpdateShiftAsync(id, operator_.Id, dto, ct);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPatch("{id:int}/close")]
    [Authorize(Roles = "Machinist,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CloseShiftAsync(int id, [FromBody] UpdateShiftRequestDto dto, CancellationToken ct = default)
    {
        var operator_ = await _userService.GetCurrentUserAsync(ct);
        var result = await _shiftService.CloseShiftAsync(id, operator_.Id, dto, ct);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("my-logs")]
    [Authorize(Roles = "Machinist,Admin")]
    [ProducesResponseType(typeof(List<ShiftLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ShiftLogDto>>> ListShiftLogsAsync(CancellationToken ct = default)
    {
        var operator_ = await _userService.GetCurrentUserAsync(ct);
        var logs = await _shiftService.ListShiftLogsAsync(operator_.Id, ct);
        return Ok(logs);
    }
}

