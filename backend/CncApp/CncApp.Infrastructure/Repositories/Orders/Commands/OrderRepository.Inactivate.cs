using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class OrderRepository : IOrderRepository
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var order = await _context.Orders.FindAsync(new object[] { id }, ct);
        if (order == null)
            return false;

        order.Inactivate(inactivatedByUserId);

        return true;
    }
}

