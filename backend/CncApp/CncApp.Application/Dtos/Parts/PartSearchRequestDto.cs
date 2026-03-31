using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Parts;

public class PartSearchRequestDto
{
    public string? PartName { get; set; }

    public string? PartNumber { get; set; }

    public string SortColumn { get; set; } = "PartName";

    public string SortDirection { get; set; } = "asc";

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 25;
}
