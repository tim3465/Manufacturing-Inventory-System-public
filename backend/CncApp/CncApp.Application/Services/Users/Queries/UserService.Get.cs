using CncApp.Application.Dtos.Users;

namespace CncApp.Application.Services.Users;

public partial class UserService
{
    public Task<UserDto?> GetAsync(int id, CancellationToken ct = default)
    {
        return GetInternalAsync(id, ct);
    }

    private async Task<UserDto?> GetInternalAsync(int id, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user == null)
        {
            return null;
        }

        return _mapper.Map<UserDto>(user);
    }
}

