using CncApp.Application.Dtos.Users;

namespace CncApp.Application.Services.Users;

public partial class UserService
{
    public async Task<List<UserDto>> ListActiveAsync(CancellationToken ct = default)
    {
        var users = await _userRepository.ListActiveAsync(ct);

        return _mapper.Map<List<UserDto>>(users);
    }
}

