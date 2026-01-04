using CncApp.Domain.Entities;

namespace CncApp.Application.Interfaces.Repositories;

public interface IStockLotAdjustmentRepository
{
    Task<StockLotAdjustment?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<StockLotAdjustment>> ListByStockLotIdAsync(int stockLotId, CancellationToken ct = default);
    Task AddAsync(StockLotAdjustment stockLotAdjustment, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

