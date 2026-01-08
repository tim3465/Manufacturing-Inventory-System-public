namespace CncApp.Infrastructure.Repositories;

public partial class UserRepository
{
    public Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

