using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class StockLotRepository
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}

