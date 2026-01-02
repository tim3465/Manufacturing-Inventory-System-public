using CncApp.Application.Dtos.Users;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.Users;

public partial class UserService
{
    /// <summary>
    /// Creates both an Identity user and a Domain user in a single operation.
    /// This is the admin-only user provisioning flow.
    /// </summary>
    /// <param name="dto">The user creation request containing email, username, password, roles, and domain user info.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created user information including both Identity and Domain UserIds.</returns>
    public async Task<CreateUserResponseDto> CreateAsync(CreateUserRequestDto dto, CancellationToken ct = default)
    {
        // Step 1: Create Identity user (email, username, password)
        var identityUserId = await _identityProvisioningService.CreateIdentityUserAsync(
            dto.Email,
            dto.UserName,
            dto.TemporaryPassword,
            ct);

        // Step 2: Assign Identity roles (roles are the source of truth for authorization)
        if (dto.Roles.Any())
        {
            await _identityProvisioningService.AssignRolesAsync(identityUserId, dto.Roles, ct);
        }

        // Step 3: Create Domain User linked via IdentityUserId
        // Note: IdentityUserId is created internally, never from client
        var currentIdentityUserId = _currentUserService.GetCurrentUserId();
        var domainUser = new User
        {
            IdentityUserId = identityUserId, // Link to Identity user
            UserName = dto.UserName,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            // Email is NOT stored in Domain User (Identity owns email as source of truth)
            // Audit fields
            CreatedDateTime = DateTimeOffset.UtcNow,
            CreatedByUserId = currentIdentityUserId
        };

        await _userRepository.AddAsync(domainUser, ct);
        await _userRepository.SaveChangesAsync(ct);

        return new CreateUserResponseDto
        {
            IdentityUserId = identityUserId,
            DomainUserId = domainUser.Id,
            Email = dto.Email,
            UserName = dto.UserName
        };
    }
}

