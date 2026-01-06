using AutoMapper;
using CncApp.Application.Dtos.StockLots;

namespace CncApp.Application.Services.StockLots;

public partial class StockLotService
{
    public async Task<StockLotDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var stockLot = await _stockLotRepository.GetByIdAsync(id, ct);
        if (stockLot == null)
            return null;

        return _mapper.Map<StockLotDto>(stockLot);
    }
}

