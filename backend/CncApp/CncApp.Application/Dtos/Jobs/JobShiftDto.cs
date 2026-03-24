namespace CncApp.Application.Dtos.Jobs;

public class JobShiftDto
{
    public int ShiftId { get; set; }
    public string MachinistName { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
}
