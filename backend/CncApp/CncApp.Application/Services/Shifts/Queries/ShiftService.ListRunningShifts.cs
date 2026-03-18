using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public async Task<List<RunningShiftDto>> ListRunningShiftsAsync(int operatorId, CancellationToken ct = default)
    {
        var shifts = await _shiftRepository.ListRunningByOperatorAsync(operatorId, ct);

        return shifts.Select(shift =>
        {
            var closedSiblings = shift.Job.Shifts
                .Where(s => !s.InactivatedDateTime.HasValue && s.StopTime != null)
                .ToList();

            return new RunningShiftDto
            {
                Id = shift.Id,
                JobId = shift.JobId,
                MachineId = shift.Job.MachineId,
                MachineSerialNumber = shift.Job.Machine?.SerialNumber ?? string.Empty,
                PartName = shift.Job.Order?.Part?.PartName ?? string.Empty,
                PartNumber = shift.Job.Order?.Part?.PartNumber ?? string.Empty,
                JobTotalPartsMade = closedSiblings.Sum(s => s.PartsMade),
                JobTotalScrap = closedSiblings.Sum(s => s.Scrap),
                JobTotalBarsConsumed = closedSiblings.Sum(s => s.BarsConsumed),
                StartTime = shift.StartTime,
                StopTime = shift.StopTime,
                PartsMade = shift.PartsMade,
                Scrap = shift.Scrap,
                BarsConsumed = shift.BarsConsumed,
                PartsPerBar = shift.PartsPerBar,
                Downtime = shift.Downtime
            };
        }).ToList();
    }
}
