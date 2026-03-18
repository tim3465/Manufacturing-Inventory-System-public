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

        if (dto.StopTime == null)
            throw new InvalidOperationException("StopTime is required to close a shift.");

        if (dto.StopTime <= dto.StartTime)
            throw new InvalidOperationException("StopTime must be after StartTime.");

        shift.StartTime = dto.StartTime;
        shift.StopTime = dto.StopTime;
        shift.PartsMade = dto.PartsMade;
        shift.Scrap = dto.Scrap;
        shift.BarsConsumed = dto.BarsConsumed;
        shift.PartsPerBar = dto.PartsPerBar;
        shift.Downtime = dto.Downtime;

        await _shiftRepository.SaveChangesAsync(ct);
        return true;
    }
}
