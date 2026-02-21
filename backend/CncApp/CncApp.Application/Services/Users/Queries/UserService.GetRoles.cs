using CncApp.Application.Dtos.Users;

namespace CncApp.Application.Services.Users;

public partial class UserService
{
    public async Task<UserRolesDto?> GetRolesAsync(int id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user == null)
        {
            return null;
        }

        var roles = await _identityProvisioningService.GetRolesAsync(user.IdentityUserId, ct);

        return new UserRolesDto
        {
            UserId = user.Id,
            Roles = roles
        };
    }
}


