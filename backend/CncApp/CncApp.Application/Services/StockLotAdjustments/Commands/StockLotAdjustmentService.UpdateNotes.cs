using AutoMapper;
using CncApp.Application.Dtos.StockLotAdjustments;

namespace CncApp.Application.Services.StockLotAdjustments;

public partial class StockLotAdjustmentService
{
    public async Task<StockLotAdjustmentDto?> UpdateNotesAsync(int id, UpdateStockLotAdjustmentNotesRequestDto dto, CancellationToken ct = default)
    {
        var stockLotAdjustment = await _stockLotAdjustmentRepository.GetByIdAsync(id, ct);
        if (stockLotAdjustment == null)
            return null;

        stockLotAdjustment.Notes = dto.Notes;
        await _stockLotAdjustmentRepository.SaveChangesAsync(ct);

        return _mapper.Map<StockLotAdjustmentDto>(stockLotAdjustment);
    }
}

