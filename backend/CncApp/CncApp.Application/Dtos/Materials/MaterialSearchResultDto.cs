namespace CncApp.Application.Dtos.Materials;

public class MaterialSearchResultDto
{
    public List<MaterialDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
