using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Jobs;

public class MyJobSearchRequestDto
{
    public string? JobNumber { get; set; }
    public string? PartNumber { get; set; }
    public string? MachineName { get; set; }
    public string? Status { get; set; } // "In Progress" or "Completed"
    public string SortColumn { get; set; } = "JobNumber";
    public string SortDirection { get; set; } = "asc";
    [Range(1, int.MaxValue)] public int Page { get; set; } = 1;
    [Range(1, 100)]          public int PageSize { get; set; } = 25;
}
