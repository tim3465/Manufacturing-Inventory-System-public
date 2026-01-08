using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftRepository
{
    public async Task<Shift?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Shifts.FindAsync(new object[] { id }, ct);
    }
}


