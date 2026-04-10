using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class OrderRepository : IOrderRepository
{
    public async Task<List<Order>> ListActiveWithDetailsAsync(CancellationToken ct = default)
    {
        return await _context.Orders
            .Where(o => !o.InactivatedDateTime.HasValue)
            .Include(o => o.Customer)
            .Include(o => o.Part)
            .Include(o => o.Jobs)
                .ThenInclude(j => j.Shifts)
            .ToListAsync(ct);
    }
}
