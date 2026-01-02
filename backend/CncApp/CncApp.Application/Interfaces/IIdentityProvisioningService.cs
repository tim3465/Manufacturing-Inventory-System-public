namespace CncApp.Application.Interfaces;

/// <summary>
/// Service for provisioning Identity users (abstraction to avoid Application layer depending on ASP.NET Core Identity).
/// </summary>
public interface IIdentityProvisioningService
{
    /// <summary>
    /// Creates a new Identity user with the specified email, username, and password.
    /// </summary>
    /// <param name="email">Email address for the Identity user.</param>
    /// <param name="userName">Username for the Identity user.</param>
    /// <param name="password">Temporary password for the Identity user.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created Identity UserId.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user creation fails.</exception>
    Task<int> CreateIdentityUserAsync(string email, string userName, string password, CancellationToken ct = default);

    /// <summary>
    /// Assigns Identity roles to an Identity user.
    /// </summary>
    /// <param name="identityUserId">The Identity UserId.</param>
    /// <param name="roles">List of role names to assign.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <exception cref="InvalidOperationException">Thrown when role assignment fails.</exception>
    Task AssignRolesAsync(int identityUserId, IEnumerable<string> roles, CancellationToken ct = default);
}

