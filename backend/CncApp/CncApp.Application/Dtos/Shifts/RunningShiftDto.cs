namespace CncApp.Application.Dtos.Shifts;

public class RunningShiftDto
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int MachineId { get; set; }
    public string MachineSerialNumber { get; set; } = string.Empty;
    public string PartName { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public int JobTotalPartsMade { get; set; }
    public int JobTotalScrap { get; set; }
    public int JobTotalBarsConsumed { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? StopTime { get; set; }
    public int PartsMade { get; set; }
    public int Scrap { get; set; }
    public int BarsConsumed { get; set; }
    public int? PartsPerBar { get; set; }
    public TimeSpan? Downtime { get; set; }
}
