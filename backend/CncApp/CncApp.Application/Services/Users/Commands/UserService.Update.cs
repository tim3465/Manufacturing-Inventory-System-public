using CncApp.Application.Dtos.Users;

namespace CncApp.Application.Services.Users;

public partial class UserService
{
    public async Task<bool> UpdateRolesAsync(int id, UpdateUserRolesRequestDto dto, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user == null)
        {
            return false;
        }

        await _identityProvisioningService.AssignRolesAsync(user.IdentityUserId, dto.Roles, ct);

        return true;
    }
}

