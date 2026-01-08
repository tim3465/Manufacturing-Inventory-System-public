namespace CncApp.Application.Dtos.Parts;

public class UpdatePartRequestDto
{
    public TimeSpan? ApproxPartCycleTime { get; set; }

    public int? CheckPerPart { get; set; }
}

