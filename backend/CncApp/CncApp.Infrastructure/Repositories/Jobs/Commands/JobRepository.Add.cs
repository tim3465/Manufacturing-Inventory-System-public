using CncApp.Domain.Entities;

namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository
{
    public async Task AddAsync(Job job, CancellationToken ct = default)
    {
        await _context.Jobs.AddAsync(job, ct);
    }
}

