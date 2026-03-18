using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository
{
    public async Task<Job?> GetActiveJobByMachineAsync(int machineId, CancellationToken ct = default)
    {
        return await _context.Jobs
            .Where(j => j.MachineId == machineId
                && !j.InactivatedDateTime.HasValue
                && j.StartedDateTime.HasValue
                && j.EndedDateTime == null)
            .FirstOrDefaultAsync(ct);
    }
}
