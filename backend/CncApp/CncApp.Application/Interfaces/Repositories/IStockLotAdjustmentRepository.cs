using CncApp.Domain.Entities;

namespace CncApp.Application.Interfaces.Repositories;

public interface IStockLotAdjustmentRepository
{
    Task<StockLotAdjustment?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<StockLotAdjustment>> ListByStockLotAsync(int stockLotId, CancellationToken ct = default);
    Task<List<StockLotAdjustment>> ListAllAsync(CancellationToken ct = default);
    Task AddAsync(StockLotAdjustment stockLotAdjustment, CancellationToken ct = default);
    Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

