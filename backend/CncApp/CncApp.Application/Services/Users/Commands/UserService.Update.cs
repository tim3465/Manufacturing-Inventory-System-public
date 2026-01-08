using CncApp.Application.Dtos.Users;

namespace CncApp.Application.Services.Users;

public partial class UserService
{
    public Task<bool> UpdateRolesAsync(int id, UpdateUserRolesRequestDto dto, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

