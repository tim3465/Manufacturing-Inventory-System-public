namespace CncApp.Application.Dtos.Orders;

public class OrderProductionSearchResultDto
{
    public List<OrderProductionDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
