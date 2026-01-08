using CncApp.Domain.Entities;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftRepository
{
    public async Task AddAsync(Shift shift, CancellationToken ct = default)
    {
        await _context.Shifts.AddAsync(shift, ct);
    }
}


