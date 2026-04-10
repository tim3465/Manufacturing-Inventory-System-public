namespace CncApp.Application.Dtos.Parts;

public class PartSearchResultDto
{
    public List<PartDto> Items { get; set; } = new();

    public int TotalCount { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }
}
