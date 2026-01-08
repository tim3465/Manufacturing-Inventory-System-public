using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public async Task<ShiftDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var shift = await _shiftRepository.GetByIdAsync(id, ct);
        if (shift == null)
        {
            return null;
        }

        return _mapper.Map<ShiftDto>(shift);
    }
}


