using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public async Task<bool> CloseShiftAsync(int shiftId, int operatorId, UpdateShiftRequestDto dto, CancellationToken ct = default)
    {
        var shift = await _shiftRepository.GetByIdAsync(shiftId, ct);
        if (shift == null || shift.InactivatedDateTime.HasValue)
            return false;

        if (shift.OperatorId != operatorId)
            throw new InvalidOperationException("You can only close your own shifts.");

        var closedAt = DateTime.UtcNow;

        shift.StartTime = dto.StartTime;
        shift.StopTime = closedAt;
        shift.PartsMade = dto.PartsMade;
        shift.BarsConsumed = dto.BarsConsumed;
        shift.PartsPerBar = dto.PartsPerBar;

        await _shiftRepository.SaveChangesAsync(ct);
        return true;
    }
}
