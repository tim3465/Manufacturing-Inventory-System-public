namespace CncApp.Application.Dtos.Jobs;

public class MyJobListItemDto
{
    public int Id { get; set; }
    public string JobNumber { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string PartName { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public DateTimeOffset? EndedDateTime { get; set; }
}
