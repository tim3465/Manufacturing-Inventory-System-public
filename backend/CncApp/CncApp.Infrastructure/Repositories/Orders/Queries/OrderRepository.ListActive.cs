using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class OrderRepository : IOrderRepository
{
    public async Task<List<Order>> ListActiveAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

