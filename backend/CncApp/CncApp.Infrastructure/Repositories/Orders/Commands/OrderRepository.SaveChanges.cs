using CncApp.Application.Interfaces.Repositories;
using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class OrderRepository : IOrderRepository
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}

