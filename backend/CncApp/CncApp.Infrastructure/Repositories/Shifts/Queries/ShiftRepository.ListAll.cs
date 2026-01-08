using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftRepository
{
    public async Task<List<Shift>> ListAllAsync(CancellationToken ct = default)
    {
        return await _context.Shifts.ToListAsync(ct);
    }
}


