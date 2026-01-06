using AutoMapper;
using CncApp.Application.Dtos.StockLots;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.StockLots;

public partial class StockLotService
{
    public async Task<bool> UpdateAsync(int id, UpdateStockLotRequestDto dto, CancellationToken ct = default)
    {
        var stockLot = await _stockLotRepository.GetByIdAsync(id, ct);
        if (stockLot == null)
            return false;

        // Update metadata only - AmountOfBars is excluded (quantity changes via StockLotAdjustments)
        _mapper.Map(dto, stockLot);

        await _stockLotRepository.SaveChangesAsync(ct);

        return true;
    }
}

