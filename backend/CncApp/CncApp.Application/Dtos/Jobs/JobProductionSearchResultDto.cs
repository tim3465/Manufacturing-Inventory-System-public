namespace CncApp.Application.Dtos.Jobs;

public class JobProductionSearchResultDto
{
    public List<JobProductionDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
