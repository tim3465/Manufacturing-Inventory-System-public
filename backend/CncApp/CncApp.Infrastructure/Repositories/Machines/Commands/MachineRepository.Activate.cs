using CncApp.Application.Interfaces.Repositories;
using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class MachineRepository : IMachineRepository
{
    public async Task<bool> ActivateAsync(int id, CancellationToken ct = default)
    {
        var machine = await _context.Machines.FindAsync(new object[] { id }, ct);
        if (machine == null)
            return false;

        machine.InactivatedDateTime = null;
        machine.InactivatedByUserId = null;

        return true;
    }
}
