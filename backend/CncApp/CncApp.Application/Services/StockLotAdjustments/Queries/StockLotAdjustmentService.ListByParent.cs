using CncApp.Application.Dtos.StockLotAdjustments;

namespace CncApp.Application.Services.StockLotAdjustments;

public partial class StockLotAdjustmentService
{
    public async Task<List<StockLotAdjustmentDto>> ListByStockLotAsync(int parentId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

