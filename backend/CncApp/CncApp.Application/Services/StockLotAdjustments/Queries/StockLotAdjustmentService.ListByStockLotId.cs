using CncApp.Application.Dtos.StockLotAdjustments;

namespace CncApp.Application.Services.StockLotAdjustments;

public partial class StockLotAdjustmentService
{
    // TODO: clarify - Ledger tables typically use command-centric methods
    // Consider if this should be ListByStockLotIdAsync or similar pattern
    public async Task<List<StockLotAdjustmentDto>> ListByStockLotIdAsync(int stockLotId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

