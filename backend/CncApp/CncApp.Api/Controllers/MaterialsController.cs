using CncApp.Application.Dtos.Materials;
using CncApp.Application.Services.Materials;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for managing materials.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MaterialsController : ControllerBase
{
    private readonly MaterialService _materialService;

    public MaterialsController(MaterialService materialService)
    {
        _materialService = materialService;
    }

    /// <summary>
    /// Creates a new material.
    /// </summary>
    /// <param name="dto">The material creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created material ID with Location header.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateAsync(
        [FromBody] CreateMaterialRequestDto dto,
        CancellationToken ct = default)
    {
        var id = await _materialService.CreateAsync(dto, ct);
        return CreatedAtRoute(routeName: "GetMaterial", routeValues: new { id }, value: new { id });
    }

    /// <summary>
    /// Updates a material by ID (metadata-only: HeatNumber, MaterialName).
    /// </summary>
    /// <param name="id">The material ID.</param>
    /// <param name="dto">The material update request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated material if found, otherwise 404.</returns>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(MaterialDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MaterialDto>> UpdateAsync(
        int id,
        [FromBody] UpdateMaterialRequestDto dto,
        CancellationToken ct = default)
    {
        var material = await _materialService.UpdateAsync(id, dto, ct);
        if (material == null)
        {
            return NotFound();
        }

        return Ok(material);
    }

    /// <summary>
    /// Gets all active materials.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of all active materials.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<MaterialDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MaterialDto>>> ListAsync(CancellationToken ct = default)
    {
        var materials = await _materialService.ListActiveAsync(ct);
        return Ok(materials);
    }

    /// <summary>
    /// Gets all materials.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of all materials.</returns>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<MaterialDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MaterialDto>>> ListAllAsync(CancellationToken ct = default)
    {
        var materials = await _materialService.ListAllAsync(ct);
        return Ok(materials);
    }

    /// <summary>
    /// Gets a material by ID.
    /// </summary>
    /// <param name="id">The material ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The material if found, otherwise 404.</returns>
    [HttpGet("{id:int}", Name = "GetMaterial")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(MaterialDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MaterialDto>> GetAsync(int id, CancellationToken ct = default)
    {
        var material = await _materialService.GetAsync(id, ct);
        if (material == null)
        {
            return NotFound();
        }

        return Ok(material);
    }

    /// <summary>
    /// Inactivates (soft deletes) a material by ID.
    /// </summary>
    /// <param name="id">The material ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>204 NoContent if successful, otherwise 404.</returns>
    [HttpPatch("{id:int}/inactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> InactivateAsync(int id, CancellationToken ct = default)
    {
        var result = await _materialService.InactivateAsync(id, null, ct);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}

