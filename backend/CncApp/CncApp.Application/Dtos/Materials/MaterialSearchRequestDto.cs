using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Materials;

public class MaterialSearchRequestDto
{
    public string? HeatNumber { get; set; }
    public string? MaterialName { get; set; }
    public string SortColumn { get; set; } = "HeatNumber";
    public string SortDirection { get; set; } = "asc";
    [Range(1, int.MaxValue)] public int Page { get; set; } = 1;
    [Range(1, 100)]          public int PageSize { get; set; } = 25;
}
