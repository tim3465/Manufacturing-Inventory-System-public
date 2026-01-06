using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class StockLotRepository
{
    public async Task AddAsync(StockLot stockLot, CancellationToken ct = default)
    {
        await _context.StockLots.AddAsync(stockLot, ct);
    }
}

