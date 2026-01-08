using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository
{
    public async Task<List<Job>> ListActiveAsync(CancellationToken ct = default)
    {
        return await _context.Jobs
            .Where(j => !j.InactivatedDateTime.HasValue)
            .OrderBy(j => j.CreatedDateTime)
            .ToListAsync(ct);
    }
}

