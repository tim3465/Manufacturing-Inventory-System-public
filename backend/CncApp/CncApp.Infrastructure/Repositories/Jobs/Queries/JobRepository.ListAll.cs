using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository : IJobRepository
{
    public async Task<List<Job>> ListAllAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

