using CncApp.Domain.Entities;

namespace CncApp.Application.Interfaces.Repositories;

public interface IStockLotRepository
{
    Task<StockLot?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<StockLot>> ListActiveAsync(CancellationToken ct = default);
    Task<List<StockLot>> ListAllAsync(CancellationToken ct = default);
    Task AddAsync(StockLot stockLot, CancellationToken ct = default);
    Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
