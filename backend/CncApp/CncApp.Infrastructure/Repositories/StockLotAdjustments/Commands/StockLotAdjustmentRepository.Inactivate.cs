using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class StockLotAdjustmentRepository
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var stockLotAdjustment = await _context.StockLotAdjustments.FindAsync(new object[] { id }, ct);
        if (stockLotAdjustment == null)
            return false;

        stockLotAdjustment.InactivatedDateTime = DateTimeOffset.UtcNow;
        stockLotAdjustment.InactivatedByUserId = inactivatedByUserId;

        return true;
    }
}

