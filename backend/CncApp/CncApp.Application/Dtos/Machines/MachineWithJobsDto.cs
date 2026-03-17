namespace CncApp.Application.Dtos.Machines;

public class MachineWithJobsDto
{
    public int Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string ModelNumber { get; set; } = string.Empty;
    public List<MachineJobSummaryDto> Jobs { get; set; } = new();
}
