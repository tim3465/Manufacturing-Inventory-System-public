using CncApp.Domain.Enums;

namespace CncApp.Application.Dtos.Jobs;

public class JobReportIssueLogDto
{
    public int Id { get; set; }
    public int ShiftId { get; set; }
    public string OperatorName { get; set; } = string.Empty;
    public DateTimeOffset CreatedDateTime { get; set; }
    public IssueTypeEnum IssueType { get; set; }
    public string Description { get; set; } = string.Empty;
    public int ScrapQuantity { get; set; }
    public TimeSpan? Downtime { get; set; }
}
