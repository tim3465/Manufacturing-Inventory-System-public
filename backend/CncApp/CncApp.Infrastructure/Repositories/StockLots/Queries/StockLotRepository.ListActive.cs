using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class StockLotRepository
{
    public async Task<List<StockLot>> ListActiveAsync(CancellationToken ct = default)
    {
        return await _context.StockLots.Where(sl => !sl.InactivatedDateTime.HasValue).ToListAsync(ct);
    }
}

