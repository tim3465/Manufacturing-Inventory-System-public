using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Orders;

public class OrderProductionSearchRequestDto
{
    public string? CustomerName { get; set; }
    public string? PartName { get; set; }
    public string? PartNumber { get; set; }
    public string SortColumn { get; set; } = "CustomerName";
    public string SortDirection { get; set; } = "asc";
    [Range(1, int.MaxValue)] public int Page { get; set; } = 1;
    [Range(1, 100)]          public int PageSize { get; set; } = 25;
}
