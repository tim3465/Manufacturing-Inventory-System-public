using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftRepository
{
    public async Task<List<Shift>> ListStartedTodayAsync(DateOnly today, CancellationToken ct = default)
    {
        var todayStart = today.ToDateTime(TimeOnly.MinValue);
        var todayEnd = today.ToDateTime(TimeOnly.MaxValue);

        return await _context.Shifts
            .Where(s => !s.InactivatedDateTime.HasValue
                && s.StartTime >= todayStart
                && s.StartTime <= todayEnd)
            .ToListAsync(ct);
    }
}
