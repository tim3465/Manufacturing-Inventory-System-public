using AutoMapper;
using CncApp.Application.Dtos.StockLotAdjustments;

namespace CncApp.Application.Services.StockLotAdjustments;

public partial class StockLotAdjustmentService
{
    public async Task<List<StockLotAdjustmentDto>> ListAllAsync(CancellationToken ct = default)
    {
        var stockLotAdjustments = await _stockLotAdjustmentRepository.ListAllAsync(ct);
        return _mapper.Map<List<StockLotAdjustmentDto>>(stockLotAdjustments);
    }
}

