using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public async Task<List<ShiftDto>> ListProductionAsync(CancellationToken ct = default)
    {
        var shifts = await _shiftRepository.ListProductionAsync(ct);
        return _mapper.Map<List<ShiftDto>>(shifts);
    }
}
