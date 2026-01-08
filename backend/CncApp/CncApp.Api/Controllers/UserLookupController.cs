using CncApp.Application.Dtos.Users;
using CncApp.Application.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserLookupController : ControllerBase
{
    private readonly UserService _userService;

    public UserLookupController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Gets active users (operators). Active means InactivatedDateTime is null.
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserDto>>> ListAsync(CancellationToken ct = default)
    {
        var users = await _userService.ListActiveAsync(ct);
        return Ok(users);
    }

    /// <summary>
    /// Gets all users (including inactive). Admin-only.
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
    /// Gets a user by id.
    /// </summary>
    [HttpGet("{id:int}", Name = "GetUser")]
    [Authorize]
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
}

