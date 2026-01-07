using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class StockLotAdjustmentRepository
{
    public async Task<StockLotAdjustment?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.StockLotAdjustments.FindAsync(new object[] { id }, ct);
    }
}

