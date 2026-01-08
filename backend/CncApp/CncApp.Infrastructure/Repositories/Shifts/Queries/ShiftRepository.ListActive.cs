using System.Linq;
using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftRepository
{
    public async Task<List<Shift>> ListActiveAsync(CancellationToken ct = default)
    {
        return await _context.Shifts
            .Where(s => !s.InactivatedDateTime.HasValue)
            .ToListAsync(ct);
    }
}


