using CncApp.Application.Dtos.Users;
using CncApp.Application.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for user management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    // Conventions:
    // - All deletes are soft deletes via PATCH /{id}/inactivate.
    // - GET /all endpoints are Admin only and include inactive records.
    // - Most resources allow anonymous read access; Users requires authentication.

    /// <summary>
    /// Lists active users (operators). Active when InactivatedDateTime is null.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserDto>>> ListAsync(CancellationToken ct = default)
    {
        var users = await _userService.ListActiveAsync(ct);
        return Ok(users);
    }

    /// <summary>
    /// Gets a user by id.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetUser")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetAsync(int id, CancellationToken ct = default)
    {
        var user = await _userService.GetAsync(id, ct);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Gets Identity roles assigned to a user by domain user id (Admin-only).
    /// </summary>
    [HttpGet("{id:int}/roles")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserRolesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserRolesDto>> GetRolesAsync(int id, CancellationToken ct = default)
    {
        var userRoles = await _userService.GetRolesAsync(id, ct);
        if (userRoles == null)
        {
            return NotFound();
        }

        return Ok(userRoles);
    }

    /// <summary>
    /// Lists all users including inactive (Admin-only).
    /// </summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserDto>>> ListAllAsync(CancellationToken ct = default)
    {
        var users = await _userService.ListAllAsync(ct);
        return Ok(users);
    }

    /// <summary>
    /// Creates a new user (both Identity and Domain user) in a single operation.
    /// This is an admin-only endpoint - no self-registration is allowed.
    /// </summary>
    /// <param name="dto">The user creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created user information.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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

