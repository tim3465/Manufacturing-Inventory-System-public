using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class MachineRepository : IMachineRepository
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var machine = await _context.Machines.FindAsync(new object[] { id }, ct);
        if (machine == null)
            return false;

        machine.InactivatedDateTime = DateTimeOffset.UtcNow;

        return true;
    }
}

