using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftRepository
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var shift = await _context.Shifts.FindAsync(new object[] { id }, ct);
        if (shift == null)
        {
            return false;
        }

        shift.Inactivate(inactivatedByUserId);
        return true;
    }
}


