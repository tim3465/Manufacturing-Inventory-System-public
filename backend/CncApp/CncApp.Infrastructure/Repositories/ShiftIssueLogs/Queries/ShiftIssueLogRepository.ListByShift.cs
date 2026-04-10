using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftIssueLogRepository
{
    public async Task<List<ShiftIssueLog>> ListByShiftAsync(int shiftId, CancellationToken ct = default)
    {
        return await _context.ShiftIssueLogs
            .Where(x => x.ShiftId == shiftId)
            .OrderBy(x => x.CreatedDateTime)
            .ToListAsync(ct);
    }
}
