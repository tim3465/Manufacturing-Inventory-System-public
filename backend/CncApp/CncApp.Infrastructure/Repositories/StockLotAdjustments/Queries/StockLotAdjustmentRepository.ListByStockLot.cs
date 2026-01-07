using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class StockLotAdjustmentRepository
{
    public async Task<List<StockLotAdjustment>> ListByStockLotAsync(int stockLotId, CancellationToken ct = default)
    {
        return await _context.StockLotAdjustments
            .Where(sla => sla.StockLotId == stockLotId && !sla.InactivatedDateTime.HasValue)
            .OrderBy(sla => sla.CreatedDateTime)
            .ToListAsync(ct);
    }
}

