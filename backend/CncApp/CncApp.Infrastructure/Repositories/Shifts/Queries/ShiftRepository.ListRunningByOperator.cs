using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftRepository
{
    public async Task<List<Shift>> ListRunningByOperatorAsync(int operatorId, CancellationToken ct = default)
    {
        return await _context.Shifts
            .Where(s => s.OperatorId == operatorId && !s.InactivatedDateTime.HasValue && s.StopTime == null)
            .Include(s => s.Job)
                .ThenInclude(j => j.Machine)
            .Include(s => s.Job)
                .ThenInclude(j => j.Order)
                    .ThenInclude(o => o.Part)
            .Include(s => s.Job)
                .ThenInclude(j => j.Shifts.Where(sibling => !sibling.InactivatedDateTime.HasValue && sibling.StopTime != null))
            .OrderBy(s => s.StartTime)
            .ToListAsync(ct);
    }
}
