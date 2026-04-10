using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Jobs;

public class JobProductionSearchRequestDto
{
    public DateOnly? DueDateFrom { get; set; }
    public DateOnly? DueDateTo { get; set; }
    public string? OrderNumber { get; set; }
    public string? PartName { get; set; }
    public string? PartNumber { get; set; }
    public string? MachineName { get; set; }
    public string? LotNumber { get; set; }
    public string SortColumn { get; set; } = "DueDate";
    public string SortDirection { get; set; } = "asc";
    [Range(1, int.MaxValue)] public int Page { get; set; } = 1;
    [Range(1, 100)]          public int PageSize { get; set; } = 25;
}
