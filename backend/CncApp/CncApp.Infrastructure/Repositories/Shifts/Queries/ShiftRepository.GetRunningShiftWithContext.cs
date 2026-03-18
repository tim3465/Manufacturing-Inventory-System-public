using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftRepository
{
    public async Task<Shift?> GetRunningShiftWithContextAsync(int shiftId, CancellationToken ct = default)
    {
        return await _context.Shifts
            .Where(s => s.Id == shiftId && !s.InactivatedDateTime.HasValue)
            .Include(s => s.Job)
                .ThenInclude(j => j.Machine)
            .Include(s => s.Job)
                .ThenInclude(j => j.Order)
                    .ThenInclude(o => o.Part)
            .Include(s => s.Job)
                .ThenInclude(j => j.Shifts.Where(sibling => !sibling.InactivatedDateTime.HasValue && sibling.StopTime != null))
            .FirstOrDefaultAsync(ct);
    }
}
