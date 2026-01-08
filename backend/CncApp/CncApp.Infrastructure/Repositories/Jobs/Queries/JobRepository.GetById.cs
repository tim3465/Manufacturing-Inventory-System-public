using CncApp.Domain.Entities;

namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository
{
    public async Task<Job?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Jobs.FindAsync(new object[] { id }, ct);
    }
}

