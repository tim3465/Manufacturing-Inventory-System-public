using AutoMapper;
using CncApp.Application.Dtos.StockLotAdjustments;

namespace CncApp.Application.Services.StockLotAdjustments;

public partial class StockLotAdjustmentService
{
    public async Task<List<StockLotAdjustmentDto>> ListByStockLotAsync(int stockLotId, CancellationToken ct = default)
    {
        var stockLotAdjustments = await _stockLotAdjustmentRepository.ListByStockLotAsync(stockLotId, ct);
        return _mapper.Map<List<StockLotAdjustmentDto>>(stockLotAdjustments);
    }
}

