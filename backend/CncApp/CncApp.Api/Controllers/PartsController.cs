using CncApp.Application.Dtos.Parts;
using CncApp.Application.Services.Parts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for managing parts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly PartService _partService;

    public PartsController(PartService partService)
    {
        _partService = partService;
    }

    /// <summary>
    /// Creates a new part.
    /// </summary>
    /// <param name="dto">The part creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created part DTO with Location header.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PartDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PartDto>> CreateAsync(
        [FromBody] CreatePartRequestDto dto,
        CancellationToken ct = default)
    {
        var id = await _partService.CreateAsync(dto, ct);
        var part = await _partService.GetAsync(id, ct);
        return CreatedAtRoute(routeName: "GetPart", routeValues: new { id }, value: part);
    }

    /// <summary>
    /// Updates a part by ID (metadata only).
    /// </summary>
    /// <param name="id">The part ID.</param>
    /// <param name="dto">The part update request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated part DTO if found, otherwise 404.</returns>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PartDto>> UpdateAsync(
        int id,
        [FromBody] UpdatePartRequestDto dto,
        CancellationToken ct = default)
    {
        var part = await _partService.UpdateAsync(id, dto, ct);
        if (part == null)
        {
            return NotFound();
        }

        return Ok(part);
    }

    /// <summary>
    /// Inactivates (soft deletes) a part by ID.
    /// </summary>
    /// <param name="id">The part ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>204 NoContent if successful, otherwise 404.</returns>
    [HttpPatch("{id:int}/inactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> InactivateAsync(int id, CancellationToken ct = default)
    {
        var result = await _partService.InactivateAsync(id, null, ct);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Gets all active parts.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of all active parts.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<PartDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PartDto>>> ListAsync(CancellationToken ct = default)
    {
        var parts = await _partService.ListActiveAsync(ct);
        return Ok(parts);
    }

    /// <summary>
    /// Gets all parts (including inactive).
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of all parts.</returns>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<PartDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PartDto>>> ListAllAsync(CancellationToken ct = default)
    {
        var parts = await _partService.ListAllAsync(ct);
        return Ok(parts);
    }

    /// <summary>
    /// Gets a part by ID.
    /// </summary>
    /// <param name="id">The part ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The part if found, otherwise 404.</returns>
    [HttpGet("{id:int}", Name = "GetPart")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PartDto>> GetAsync(int id, CancellationToken ct = default)
    {
        var part = await _partService.GetAsync(id, ct);
        if (part == null)
        {
            return NotFound();
        }

        return Ok(part);
    }
}

