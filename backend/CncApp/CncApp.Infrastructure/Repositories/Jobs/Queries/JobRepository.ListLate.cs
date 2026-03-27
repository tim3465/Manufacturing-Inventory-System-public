using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository
{
    public async Task<List<Job>> ListLateAsync(DateOnly today, CancellationToken ct = default)
    {
        return await _context.Jobs
            .Where(j => !j.InactivatedDateTime.HasValue
                && j.StartedDateTime.HasValue
                && !j.EndedDateTime.HasValue
                && j.DueDate < today)
            .ToListAsync(ct);
    }
}
