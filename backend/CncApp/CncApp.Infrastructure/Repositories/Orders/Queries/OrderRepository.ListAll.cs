using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class OrderRepository : IOrderRepository
{
    public async Task<List<Order>> ListAllAsync(CancellationToken ct = default)
    {
        return await _context.Orders.ToListAsync(ct);
    }
}

