using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository : IJobRepository
{
    public async Task<Job?> GetByIdWithShiftsAsync(int id, CancellationToken ct = default)
    {
        return await _context.Jobs
            .Where(j => !j.InactivatedDateTime.HasValue)
            .Include(j => j.Machine)
            .Include(j => j.Order)
                .ThenInclude(o => o.Part)
            .Include(j => j.Shifts.Where(s => !s.InactivatedDateTime.HasValue))
                .ThenInclude(s => s.Operator)
            .Include(j => j.Shifts.Where(s => !s.InactivatedDateTime.HasValue))
                .ThenInclude(s => s.ShiftIssueLogs.Where(l => !l.InactivatedDateTime.HasValue))
            .FirstOrDefaultAsync(j => j.Id == id, ct);
    }
}
