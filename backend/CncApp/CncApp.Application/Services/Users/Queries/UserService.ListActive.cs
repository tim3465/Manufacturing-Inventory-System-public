using CncApp.Application.Dtos.Users;

namespace CncApp.Application.Services.Users;

public partial class UserService
{
    public Task<List<UserDto>> ListActiveAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

