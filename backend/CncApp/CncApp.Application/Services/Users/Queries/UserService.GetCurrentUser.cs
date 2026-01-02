using CncApp.Domain.Entities;

namespace CncApp.Application.Services.Users;

public partial class UserService
{
    /// <summary>
    /// Resolves the Domain User for the current authenticated Identity user.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The Domain User for the current authenticated Identity user.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no Domain User exists for the current Identity user.</exception>
    public async Task<User> GetCurrentUserAsync(CancellationToken ct = default)
    {
        // Get the current authenticated Identity UserId
        var identityUserId = _currentUserService.GetCurrentUserId();

        // Resolve Domain User by IdentityUserId
        var domainUser = await _userRepository.GetByIdentityUserIdAsync(identityUserId, ct);

        if (domainUser == null)
        {
            throw new InvalidOperationException(
                $"No Domain User found for the current authenticated Identity user (IdentityUserId: {identityUserId}). " +
                "Domain User must be provisioned by an administrator before use.");
        }

        return domainUser;
    }
}

