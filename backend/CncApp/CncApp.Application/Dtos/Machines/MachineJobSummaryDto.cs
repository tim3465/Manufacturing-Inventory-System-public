namespace CncApp.Application.Dtos.Machines;

public class MachineJobSummaryDto
{
    public int Id { get; set; }
    public string PartNumber { get; set; } = string.Empty;
    public DateOnly DueDate { get; set; }
    public string? LotNumber { get; set; }
    public DateTimeOffset? StartedDateTime { get; set; }
    public int BarsInJob { get; set; }
    public int BarAmountPlanned { get; set; }
}
