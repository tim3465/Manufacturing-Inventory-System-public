namespace CncApp.Application.Dtos.CloseJob;

public class CloseJobResponseDto
{
    public int JobId { get; set; }
    public int ShiftId { get; set; }
    public DateTimeOffset JobEndedDateTime { get; set; }
}
