using CncApp.Domain.Entities;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftIssueLogRepository
{
    public async Task AddAsync(ShiftIssueLog shiftIssueLog, CancellationToken ct = default)
    {
        await _context.ShiftIssueLogs.AddAsync(shiftIssueLog, ct);
    }
}
