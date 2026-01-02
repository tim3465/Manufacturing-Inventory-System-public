using System.IdentityModel.Tokens.Jwt;
using CncApp.Application.Interfaces;
using Microsoft.AspNetCore.Http;


namespace CncApp.Infrastructure.Services;

/// <summary>
/// Implementation of ICurrentUserService that extracts the Identity UserId from the JWT sub claim.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public int GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        if (httpContext == null)
        {
            throw new UnauthorizedAccessException("No HTTP context available. This service must be used within an HTTP request.");
        }

        if (!httpContext.User.Identity?.IsAuthenticated ?? true)
        {
            throw new UnauthorizedAccessException("No authenticated user found. Ensure [Authorize] attribute is applied.");
        }

        var subClaim = httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub);
        
        if (subClaim == null || string.IsNullOrWhiteSpace(subClaim.Value))
        {
            throw new UnauthorizedAccessException("JWT token is missing the 'sub' claim (Identity UserId).");
        }

        if (!int.TryParse(subClaim.Value, out var userId))
        {
            throw new InvalidOperationException($"Invalid Identity UserId format in JWT token: '{subClaim.Value}'. Expected an integer.");
        }

        return userId;
    }
}

