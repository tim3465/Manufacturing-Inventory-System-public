using CncApp.Application.Dtos.Users;

namespace CncApp.Application.Services.Users;

public partial class UserService
{
    public Task<UserDto?> GetAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

