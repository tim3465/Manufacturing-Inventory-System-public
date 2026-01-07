using AutoMapper;
using CncApp.Application.Dtos.StockLotAdjustments;

namespace CncApp.Application.Services.StockLotAdjustments;

public partial class StockLotAdjustmentService
{
    public async Task<StockLotAdjustmentDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var stockLotAdjustment = await _stockLotAdjustmentRepository.GetByIdAsync(id, ct);
        if (stockLotAdjustment == null)
            return null;

        return _mapper.Map<StockLotAdjustmentDto>(stockLotAdjustment);
    }
}

