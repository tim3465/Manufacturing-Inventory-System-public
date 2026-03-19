namespace CncApp.Application.Dtos.Shifts;

public class ShiftLogDto
{
    public int Id { get; set; }
    public string MachineSerialNumber { get; set; } = string.Empty;
    public string JobNumber { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? StopTime { get; set; }
    public int PartsMade { get; set; }
    public int Scrap { get; set; }
}
