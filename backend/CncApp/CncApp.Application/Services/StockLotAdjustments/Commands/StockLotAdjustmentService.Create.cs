using AutoMapper;
using CncApp.Application.Dtos.StockLotAdjustments;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.StockLotAdjustments;

public partial class StockLotAdjustmentService
{
    public async Task<int> CreateAsync(CreateStockLotAdjustmentRequestDto dto, CancellationToken ct = default)
    {
        var stockLotAdjustment = _mapper.Map<StockLotAdjustment>(dto);

        await _stockLotAdjustmentRepository.AddAsync(stockLotAdjustment, ct);
        await _stockLotAdjustmentRepository.SaveChangesAsync(ct);

        return stockLotAdjustment.Id;
    }
}

