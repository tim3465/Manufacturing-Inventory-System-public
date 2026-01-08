using CncApp.Application.Dtos.Shifts;
using CncApp.Application.Services.Shifts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly ShiftService _shiftService;

    public ShiftsController(ShiftService shiftService)
    {
        _shiftService = shiftService;
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

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ShiftDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ShiftDto>>> ListAsync(CancellationToken ct = default)
    {
        var shifts = await _shiftService.ListActiveAsync(ct);
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
}

