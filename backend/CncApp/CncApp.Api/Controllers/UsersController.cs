using CncApp.Application.Dtos.Users;
using CncApp.Application.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for user management operations.
/// All endpoints require Admin role authorization.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Creates a new user (both Identity and Domain user) in a single operation.
    /// This is an admin-only endpoint - no self-registration is allowed.
    /// </summary>
    /// <param name="dto">The user creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created user information.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CreateUserResponseDto>> CreateAsync(
        [FromBody] CreateUserRequestDto dto,
        CancellationToken ct = default)
    {
        var result = await _userService.CreateAsync(dto, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Updates Identity roles for a user (Admin-only).
    /// </summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> UpdateRolesAsync(
        int id,
        [FromBody] UpdateUserRolesRequestDto dto,
        CancellationToken ct = default)
    {
        var result = await _userService.UpdateRolesAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Inactivates (soft-deletes) a user (Admin-only).
    /// </summary>
    [HttpPatch("{id:int}/inactivate")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> InactivateAsync(
        int id,
        CancellationToken ct = default)
    {
        var result = await _userService.InactivateAsync(id, null, ct);
        return Ok(result);
    }
}

