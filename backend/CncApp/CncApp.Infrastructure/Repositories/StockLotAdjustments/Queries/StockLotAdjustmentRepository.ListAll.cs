using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class StockLotAdjustmentRepository
{
    public async Task<List<StockLotAdjustment>> ListAllAsync(CancellationToken ct = default)
    {
        return await _context.StockLotAdjustments.ToListAsync(ct);
    }
}

