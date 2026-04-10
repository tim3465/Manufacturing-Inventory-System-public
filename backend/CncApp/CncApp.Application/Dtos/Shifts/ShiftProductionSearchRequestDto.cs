using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Shifts;

public class ShiftProductionSearchRequestDto
{
    public string? OperatorName { get; set; }
    public string? JobNumber { get; set; }
    public DateTime? StartTimeFrom { get; set; }
    public DateTime? StartTimeTo { get; set; }
    public DateTime? StopTimeFrom { get; set; }
    public DateTime? StopTimeTo { get; set; }
    public string SortColumn { get; set; } = "StartTime";
    public string SortDirection { get; set; } = "desc";
    [Range(1, int.MaxValue)] public int Page { get; set; } = 1;
    [Range(1, 100)]          public int PageSize { get; set; } = 25;
}
