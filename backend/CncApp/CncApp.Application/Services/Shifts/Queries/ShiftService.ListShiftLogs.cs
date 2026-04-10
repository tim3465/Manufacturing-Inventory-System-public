using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public async Task<List<ShiftLogDto>> ListShiftLogsAsync(int operatorId, CancellationToken ct = default)
    {
        var shifts = await _shiftRepository.ListClosedByOperatorAsync(operatorId, ct);

        return shifts.Select(shift => new ShiftLogDto
        {
            Id = shift.Id,
            MachineSerialNumber = shift.Job.Machine?.SerialNumber ?? string.Empty,
            JobNumber = shift.JobId.ToString(),
            PartNumber = shift.Job.Order?.Part?.PartNumber ?? string.Empty,
            StartTime = shift.StartTime,
            StopTime = shift.StopTime,
            PartsMade = shift.PartsMade,
            Scrap = shift.Scrap
        }).ToList();
    }
}
