using CncApp.Domain.Entities;

namespace CncApp.Infrastructure.Repositories;

public partial class UserRepository
{
    public Task<List<User>> ListAllAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

