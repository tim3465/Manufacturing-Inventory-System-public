using CncApp.Domain.Entities;

namespace CncApp.Infrastructure.Repositories;

public partial class UserRepository
{
    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

