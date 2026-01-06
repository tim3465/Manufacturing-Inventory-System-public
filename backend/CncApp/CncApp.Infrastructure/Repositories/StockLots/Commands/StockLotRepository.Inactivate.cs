using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class StockLotRepository
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var stockLot = await _context.StockLots.FindAsync(new object[] { id }, ct);
        if (stockLot == null)
            return false;

        stockLot.Inactivate(inactivatedByUserId);

        return true;
    }
}

