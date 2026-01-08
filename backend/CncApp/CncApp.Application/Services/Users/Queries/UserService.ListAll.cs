using CncApp.Application.Dtos.Users;

namespace CncApp.Application.Services.Users;

public partial class UserService
{
    public async Task<List<UserDto>> ListAllAsync(CancellationToken ct = default)
    {
        var users = await _userRepository.ListAllAsync(ct);

        return _mapper.Map<List<UserDto>>(users);
    }
}

