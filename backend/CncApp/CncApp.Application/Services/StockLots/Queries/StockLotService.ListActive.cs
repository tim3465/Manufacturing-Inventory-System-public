using AutoMapper;
using CncApp.Application.Dtos.StockLots;

namespace CncApp.Application.Services.StockLots;

public partial class StockLotService
{
    public async Task<List<StockLotDto>> ListActiveAsync(CancellationToken ct = default)
    {
        var stockLots = await _stockLotRepository.ListActiveAsync(ct);
        return _mapper.Map<List<StockLotDto>>(stockLots);
    }
}

