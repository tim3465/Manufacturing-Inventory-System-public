namespace CncApp.Application.Dtos.Parts;

public class PartDto
{
    public int Id { get; set; }

    public TimeSpan ApproxPartCycleTime { get; set; }

    public int CheckPerPart { get; set; }
}

