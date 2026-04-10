using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository : IJobRepository
{
    public async Task<List<Job>> ListActiveWithShiftsAsync(CancellationToken ct = default)
    {
        return await _context.Jobs
            .Where(j => !j.InactivatedDateTime.HasValue)
            .Include(j => j.Machine)
            .Include(j => j.Order)
                .ThenInclude(o => o.Part)
            .Include(j => j.StockLot)
            .Include(j => j.Shifts)
                .ThenInclude(s => s.Operator)
            .ToListAsync(ct);
    }
}
