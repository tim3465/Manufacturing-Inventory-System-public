using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public async Task<RunningShiftDto?> GetRunningShiftAsync(int shiftId, int operatorId, CancellationToken ct = default)
    {
        var shift = await _shiftRepository.GetRunningShiftWithContextAsync(shiftId, ct);
        if (shift == null)
            return null;

        if (shift.OperatorId != operatorId)
            return null;

        var jobShifts = shift.Job.Shifts
            .Where(s => !s.InactivatedDateTime.HasValue)
            .ToList();

        var currentAlreadyIncluded = jobShifts.Any(s => s.Id == shift.Id);

        return new RunningShiftDto
        {
            Id = shift.Id,
            JobId = shift.JobId,
            MachineId = shift.Job.MachineId,
            MachineSerialNumber = shift.Job.Machine?.SerialNumber ?? string.Empty,
            PartName = shift.Job.Order?.Part?.PartName ?? string.Empty,
            PartNumber = shift.Job.Order?.Part?.PartNumber ?? string.Empty,
            JobTotalPartsMade = jobShifts.Sum(s => s.PartsMade) + (currentAlreadyIncluded ? 0 : shift.PartsMade),
            JobTotalScrap = jobShifts.Sum(s => s.Scrap) + (currentAlreadyIncluded ? 0 : shift.Scrap),
            JobTotalBarsConsumed = jobShifts.Sum(s => s.BarsConsumed) + (currentAlreadyIncluded ? 0 : shift.BarsConsumed),
            StartTime = shift.StartTime,
            StopTime = shift.StopTime,
            PartsMade = shift.PartsMade,
            Scrap = shift.Scrap,
            BarsConsumed = shift.BarsConsumed,
            PartsPerBar = shift.PartsPerBar,
            Downtime = shift.Downtime
        };
    }
}
