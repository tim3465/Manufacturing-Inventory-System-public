namespace CncApp.Application.Dtos.Jobs;

public class MyJobSearchResultDto
{
    public List<MyJobListItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
