namespace CncApp.Application.Dtos.Jobs;

public class JobReportShiftDto
{
    public int Id { get; set; }
    public string OperatorName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? StopTime { get; set; }
    public int PartsMade { get; set; }
    public int Scrap { get; set; }
    public int BarsConsumed { get; set; }
    public int? PartsPerBar { get; set; }
    public TimeSpan? Downtime { get; set; }
}
