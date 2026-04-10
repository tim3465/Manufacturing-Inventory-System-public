using CncApp.Application.Dtos.Parts;

namespace CncApp.Application.Services.Parts;

public partial class PartService
{
    public async Task<PartSearchResultDto> SearchActiveAsync(
        PartSearchRequestDto request, CancellationToken ct = default)
    {
        var (items, totalCount) = await _partRepository.SearchActiveAsync(request, ct);
        var dtos = _mapper.Map<List<PartDto>>(items);
        return new PartSearchResultDto
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
