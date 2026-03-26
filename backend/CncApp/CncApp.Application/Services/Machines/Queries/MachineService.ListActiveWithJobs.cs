using CncApp.Application.Dtos.Machines;

namespace CncApp.Application.Services.Machines;

public partial class MachineService
{
    public async Task<List<MachineWithJobsDto>> ListActiveWithJobsAsync(CancellationToken ct = default)
    {
        var machines = await _machineRepository.ListActiveWithJobsAsync(ct);

        return machines.Select(m => new MachineWithJobsDto
        {
            Id = m.Id,
            SerialNumber = m.SerialNumber,
            ModelNumber = m.ModelNumber,
            Jobs = m.Jobs
                .OrderBy(j => j.DueDate)
                .Select(j =>
                {
                    var runningShift = j.Shifts
                        .FirstOrDefault(s => !s.InactivatedDateTime.HasValue && s.StopTime == null);
                    return new MachineJobSummaryDto
                    {
                        Id = j.Id,
                        PartNumber = j.Order?.Part?.PartNumber ?? string.Empty,
                        DueDate = j.DueDate,
                        LotNumber = j.StockLot?.LotNumber,
                        StartedDateTime = j.StartedDateTime,
                        BarsInJob = j.BarsInJob,
                        BarAmountPlanned = j.BarAmountPlanned,
                        RunningShiftId = runningShift?.Id,
                        RunningShiftOperatorId = runningShift?.OperatorId
                    };
                })
                .ToList()
        }).ToList();
    }
}
