namespace CncApp.Application.Dtos.StockLots;

public class StockLotSearchResultDto
{
    public List<StockLotSummaryDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
