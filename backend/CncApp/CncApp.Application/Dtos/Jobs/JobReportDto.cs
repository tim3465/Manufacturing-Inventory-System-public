namespace CncApp.Application.Dtos.Jobs;

public class JobReportDto
{
    // Job context
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public string PartName { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public DateOnly DueDate { get; set; }
    public DateTimeOffset? StartedDateTime { get; set; }
    public DateTimeOffset? EndedDateTime { get; set; }
    public string JobStatus { get; set; } = string.Empty;

    // Job totals (aggregated from shifts)
    public int PartAmountPlanned { get; set; }
    public int TotalPartsMade { get; set; }
    public int TotalScrap { get; set; }
    public int BarAmountPlanned { get; set; }
    public int TotalBarsConsumed { get; set; }
    public int? EstimatedPartsPerBar { get; set; }
    public decimal? ActualPartsPerBar { get; set; }
    public TimeSpan TotalDowntime { get; set; }

    // Shift history
    public List<JobReportShiftDto> Shifts { get; set; } = new();
}
