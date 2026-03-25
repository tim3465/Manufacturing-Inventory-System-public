using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.StockLots;

public class StockLotSearchRequestDto
{
    public string? LotNumber { get; set; }
    public DateTimeOffset? CheckedInFrom { get; set; }
    public DateTimeOffset? CheckedInTo { get; set; }
    public decimal? DiameterExact { get; set; }
    public decimal? DiameterMin { get; set; }
    public decimal? DiameterMax { get; set; }
    public string SortColumn { get; set; } = "CheckedInDateTime";
    public string SortDirection { get; set; } = "desc";

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 25;
}
