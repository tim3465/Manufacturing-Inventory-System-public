namespace CncApp.Application.Dtos.Parts;

public class PartDto
{
    public int Id { get; set; }

    public string PartName { get; set; } = string.Empty;

    public string PartNumber { get; set; } = string.Empty;

    public TimeSpan ApproxPartCycleTime { get; set; }

    public int CheckPerPart { get; set; }
}

