using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftRepository
{
    public async Task<List<Shift>> ListOpenWithContextAsync(CancellationToken ct = default)
    {
        return await _context.Shifts
            .Where(s => !s.InactivatedDateTime.HasValue && s.StopTime == null)
            .Include(s => s.Operator)
            .Include(s => s.Job)
                .ThenInclude(j => j.Machine)
            .Include(s => s.Job)
                .ThenInclude(j => j.Order)
                    .ThenInclude(o => o.Part)
            .ToListAsync(ct);
    }
}
