namespace CncApp.Application.Interfaces;

/// <summary>
/// Service to obtain the current authenticated Identity UserId from the JWT token.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current authenticated Identity UserId from the JWT sub claim.
    /// </summary>
    /// <returns>The Identity UserId as an integer.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when no authenticated user is present.</exception>
    int GetCurrentUserId();
}

