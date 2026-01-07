using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class StockLotAdjustmentRepository
{
    public async Task AddAsync(StockLotAdjustment stockLotAdjustment, CancellationToken ct = default)
    {
        await _context.StockLotAdjustments.AddAsync(stockLotAdjustment, ct);
    }
}

