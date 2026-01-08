namespace CncApp.Application.Dtos.Parts;

public class CreatePartRequestDto
{
    public TimeSpan ApproxPartCycleTime { get; set; }

    public int CheckPerPart { get; set; }
}

