using AutoMapper;
using CncApp.Application.Dtos.Materials;

namespace CncApp.Application.Services.Materials;

public partial class MaterialService
{
    public async Task<MaterialSearchResultDto> SearchActiveAsync(
        MaterialSearchRequestDto request, CancellationToken ct = default)
    {
        var (items, totalCount) = await _materialRepository.SearchActiveAsync(request, ct);
        var dtos = _mapper.Map<List<MaterialDto>>(items);
        return new MaterialSearchResultDto
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
