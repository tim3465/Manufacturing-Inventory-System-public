using CncApp.Application.Dtos.StockLots;

namespace CncApp.Application.Services.StockLots;

public partial class StockLotService
{
    public async Task<StockLotSearchResultDto> SearchActiveAsync(
        StockLotSearchRequestDto request, CancellationToken ct = default)
    {
        var (items, totalCount) = await _stockLotRepository.SearchActiveAsync(request, ct);
        var dtos = _mapper.Map<List<StockLotSummaryDto>>(items);
        return new StockLotSearchResultDto
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
