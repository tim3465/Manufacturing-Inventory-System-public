using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public async Task<ShiftProductionSearchResultDto> SearchProductionAsync(
        ShiftProductionSearchRequestDto request, CancellationToken ct = default)
    {
        var (items, totalCount) = await _shiftRepository.SearchProductionAsync(request, ct);
        var dtos = _mapper.Map<List<ShiftDto>>(items);

        return new ShiftProductionSearchResultDto
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
