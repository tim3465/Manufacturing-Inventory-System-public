namespace CncApp.Application.Services.Users;

public partial class UserService
{
    public Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

