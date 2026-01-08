using CncApp.Domain.Entities;

namespace CncApp.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Order>> ListActiveAsync(CancellationToken ct = default);
    Task<List<Order>> ListAllAsync(CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

