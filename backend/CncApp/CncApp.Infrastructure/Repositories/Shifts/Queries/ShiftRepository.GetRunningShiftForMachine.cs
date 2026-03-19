using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftRepository
{
    public async Task<Shift?> GetRunningShiftForMachineAsync(int machineId, CancellationToken ct = default)
    {
        return await _context.Shifts
            .Where(s => !s.InactivatedDateTime.HasValue && s.StopTime == null)
            .Include(s => s.Job)
            .FirstOrDefaultAsync(s => s.Job.MachineId == machineId, ct);
    }
}
