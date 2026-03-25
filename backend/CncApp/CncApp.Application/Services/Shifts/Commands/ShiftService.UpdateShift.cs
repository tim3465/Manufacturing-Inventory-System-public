using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public async Task<bool> UpdateShiftAsync(int shiftId, int operatorId, UpdateShiftRequestDto dto, CancellationToken ct = default)
    {
        var shift = await _shiftRepository.GetByIdAsync(shiftId, ct);
        if (shift == null || shift.InactivatedDateTime.HasValue)
            return false;

        if (shift.OperatorId != operatorId)
            throw new InvalidOperationException("You can only update your own shifts.");

        if (shift.StopTime != null)
            throw new InvalidOperationException("Cannot update a closed shift.");

        shift.StartTime = dto.StartTime;
        shift.PartsMade = dto.PartsMade;
        shift.Scrap = dto.Scrap;
        shift.BarsConsumed = dto.BarsConsumed;
        shift.PartsPerBar = dto.PartsPerBar;
        shift.Downtime = dto.Downtime;

        await _shiftRepository.SaveChangesAsync(ct);
        return true;
    }
}
