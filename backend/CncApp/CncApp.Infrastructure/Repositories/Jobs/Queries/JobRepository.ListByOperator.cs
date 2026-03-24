using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository
{
    public async Task<List<Job>> ListByOperatorAsync(int operatorId, CancellationToken ct = default)
    {
        return await _context.Jobs
            .Where(j => !j.InactivatedDateTime.HasValue
                     && j.Shifts.Any(s => s.OperatorId == operatorId && !s.InactivatedDateTime.HasValue))
            .Include(j => j.Machine)
            .Include(j => j.Order)
                .ThenInclude(o => o.Part)
            .OrderByDescending(j => j.StartedDateTime)
            .ToListAsync(ct);
    }
}
