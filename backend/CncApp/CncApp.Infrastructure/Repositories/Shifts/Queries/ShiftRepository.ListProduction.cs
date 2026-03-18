using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftRepository
{
    public async Task<List<Shift>> ListProductionAsync(CancellationToken ct = default)
    {
        return await _context.Shifts
            .Where(s => !s.InactivatedDateTime.HasValue)
            .Include(s => s.Operator)
            .Include(s => s.Job)
                .ThenInclude(j => j.Order)
                    .ThenInclude(o => o.Part)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync(ct);
    }
}
