using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class StockLotRepository
{
    public async Task<StockLot?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.StockLots.FindAsync(new object[] { id }, ct);
    }
}

