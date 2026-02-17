namespace CncApp.Application.Interfaces;

/// <summary>
/// Service for provisioning Identity users (abstraction to avoid Application layer depending on ASP.NET Core Identity).
/// </summary>
public interface IIdentityProvisioningService
{
    /// <summary>
    /// Validates that all provided roles exist in the Identity role store.
    /// This is intended to be called before user creation to prevent partial provisioning.
    /// </summary>
    /// <param name="roles">Roles to validate.</param>
    /// <exception cref="InvalidOperationException">Thrown when one or more roles do not exist.</exception>
    Task ValidateRolesExistAsync(IEnumerable<string> roles);

    /// <summary>
    /// Creates a new Identity user with the specified email, username, and password.
    /// Note: Identity UserName will be set to Email (UserName = Email).
    /// </summary>
    /// <param name="email">Email address for the Identity user (also used as UserName).</param>
    /// <param name="userName">Username parameter (ignored - Email is used as UserName).</param>
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

