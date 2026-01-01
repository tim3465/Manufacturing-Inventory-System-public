using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// TEMP: Authentication test controller. Remove in Step 8 when real auth endpoints exist.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// TEMP: Test endpoint to verify JWT authentication is working.
    /// Returns 200 if valid token is provided, 401 if not.
    /// Remove in Step 8.
    /// </summary>
    [HttpGet("ping")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Ping()
    {
        return Ok(new { message = "Authentication successful", timestamp = DateTime.UtcNow });
    }
}


