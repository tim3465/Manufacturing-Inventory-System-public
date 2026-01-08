using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public async Task<int> CreateAsync(CreateShiftRequestDto dto, CancellationToken ct = default)
    {
        var shift = _mapper.Map<Shift>(dto);

        await _shiftRepository.AddAsync(shift, ct);
        await _shiftRepository.SaveChangesAsync(ct);

        return shift.Id;
    }
}


