namespace CncApp.Application.Dtos.Shifts;

public class ShiftProductionSearchResultDto
{
    public List<ShiftDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
