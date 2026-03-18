using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class MachineRepository : IMachineRepository
{
    public async Task<List<Machine>> ListActiveWithJobsAsync(CancellationToken ct = default)
    {
        return await _context.Machines
            .Where(m => !m.InactivatedDateTime.HasValue)
            .Include(m => m.Jobs.Where(j => !j.InactivatedDateTime.HasValue && j.EndedDateTime == null))
                .ThenInclude(j => j.Order)
                    .ThenInclude(o => o.Part)
            .Include(m => m.Jobs.Where(j => !j.InactivatedDateTime.HasValue && j.EndedDateTime == null))
                .ThenInclude(j => j.StockLot)
            .ToListAsync(ct);
    }
}
