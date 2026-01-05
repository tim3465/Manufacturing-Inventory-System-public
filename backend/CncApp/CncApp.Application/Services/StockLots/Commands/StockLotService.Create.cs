using AutoMapper;
using CncApp.Application.Dtos.StockLots;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.StockLots;

public partial class StockLotService
{
    public async Task<int> CreateAsync(CreateStockLotRequestDto dto, CancellationToken ct = default)
    {
        var stockLot = _mapper.Map<StockLot>(dto);

        await _stockLotRepository.AddAsync(stockLot, ct);
        await _stockLotRepository.SaveChangesAsync(ct);

        return stockLot.Id;
    }
}

