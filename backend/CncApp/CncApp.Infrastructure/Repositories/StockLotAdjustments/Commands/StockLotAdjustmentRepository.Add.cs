using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class StockLotAdjustmentRepository : IStockLotAdjustmentRepository
{
    public async Task AddAsync(StockLotAdjustment stockLotAdjustment, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

