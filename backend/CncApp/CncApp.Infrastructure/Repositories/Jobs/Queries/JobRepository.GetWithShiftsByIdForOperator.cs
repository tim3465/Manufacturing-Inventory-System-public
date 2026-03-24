using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository
{
    public async Task<Job?> GetWithShiftsByIdForOperatorAsync(int jobId, int operatorId, CancellationToken ct = default)
    {
        return await _context.Jobs
            .Where(j => j.Id == jobId
                     && !j.InactivatedDateTime.HasValue
                     && j.Shifts.Any(s => s.OperatorId == operatorId && !s.InactivatedDateTime.HasValue))
            .Include(j => j.Shifts.Where(s => !s.InactivatedDateTime.HasValue))
                .ThenInclude(s => s.Operator)
            .FirstOrDefaultAsync(ct);
    }
}
