namespace CncApp.Application.Services.Users;

public partial class UserService
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var result = await _userRepository.InactivateAsync(id, inactivatedByUserId, ct);
        if (!result)
        {
            return false;
        }

        await _userRepository.SaveChangesAsync(ct);
        return true;
    }
}

