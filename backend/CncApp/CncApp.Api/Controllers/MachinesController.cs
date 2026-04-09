using CncApp.Application.Dtos.Machines;
using CncApp.Application.Services.Machines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for managing machines.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MachinesController : ControllerBase
{
    private readonly MachineService _machineService;

    public MachinesController(MachineService machineService)
    {
        _machineService = machineService;
    }

    // Conventions:
    // - All deletes are soft deletes via PATCH /{id}/inactivate.
    // - GET /all endpoints are Admin only and include inactive records.
    // - Most resources allow anonymous read access; Users requires authentication.

    /// <summary>
    /// Gets all active machines.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of all Active machines.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<MachineDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MachineDto>>> ListAsync(CancellationToken ct = default)
    {
        var machines = await _machineService.ListActiveAsync(ct);
        return Ok(machines);
    }

    /// <summary>
    /// Gets all active machines with their active jobs.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of active machines with their active jobs.</returns>
    [HttpGet("with-jobs")]
    [ProducesResponseType(typeof(List<MachineWithJobsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MachineWithJobsDto>>> ListWithJobsAsync(CancellationToken ct = default)
    {
        var machines = await _machineService.ListActiveWithJobsAsync(ct);
        return Ok(machines);
    }

    /// <summary>
    /// Gets a machine by ID.
    /// </summary>
    /// <param name="id">The machine ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The machine if found, otherwise 404.</returns>
    [HttpGet("{id:int}", Name = "GetMachine")]
    [ProducesResponseType(typeof(MachineDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MachineDto>> GetAsync(int id, CancellationToken ct = default)
    {
        var machine = await _machineService.GetAsync(id, ct);
        if (machine == null)
        {
            return NotFound();
        }

        return Ok(machine);
    }

    /// <summary>
    /// Gets all machines.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of all machines.</returns>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<MachineDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MachineDto>>> ListAllAsync(CancellationToken ct = default)
    {
        var machines = await _machineService.ListAllAsync(ct);
        return Ok(machines);
    }

    /// <summary>
    /// Creates a new machine.
    /// </summary>
    /// <param name="dto">The machine creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created machine ID with Location header.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateAsync(
        [FromBody] CreateMachineRequestDto dto,
        CancellationToken ct = default)
    {
        var id = await _machineService.CreateAsync(dto, ct);
        return CreatedAtRoute( routeName: "GetMachine", routeValues: new { id }, value: new { id });
    }

    /// <summary>
    /// Inactivates (soft deletes) a machine by ID.
    /// </summary>
    /// <param name="id">The machine ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>204 NoContent if successful, otherwise 404.</returns>
    [HttpPatch("{id:int}/inactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> InactivateAsync(int id, CancellationToken ct = default)
    {
        var result = await _machineService.InactivateAsync(id, null, ct);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Activates a previously inactivated machine by ID.
    /// </summary>
    /// <param name="id">The machine ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>204 NoContent if successful, otherwise 404.</returns>
    [HttpPatch("{id:int}/activate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ActivateAsync(int id, CancellationToken ct = default)
    {
        var result = await _machineService.ActivateAsync(id, ct);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}


