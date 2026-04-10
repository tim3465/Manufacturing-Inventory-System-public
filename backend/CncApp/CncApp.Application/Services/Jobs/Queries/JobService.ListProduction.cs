using CncApp.Application.Dtos.Jobs;
using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<List<JobProductionDto>> ListProductionAsync(CancellationToken ct = default)
    {
        var jobs = await _jobRepository.ListActiveWithShiftsAsync(ct);

        return jobs.Select(j =>
        {
            var partsCompleted = j.Shifts.Sum(s => s.PartsMade);
            var percentComplete = j.PartAmountPlanned > 0
                ? Math.Round((decimal)partsCompleted / j.PartAmountPlanned * 100, 1)
                : 0m;

            return new JobProductionDto
            {
                Id = j.Id,
                OrderId = j.OrderId,
                DueDate = j.DueDate,
                MachineId = j.MachineId,
                MachineName = j.Machine?.SerialNumber ?? string.Empty,
                PartAmountPlanned = j.PartAmountPlanned,
                PartName = j.Order?.Part?.PartName ?? string.Empty,
                PartNumber = j.Order?.Part?.PartNumber ?? string.Empty,
                PartsCompleted = partsCompleted,
                PercentComplete = percentComplete,
                StockLotId = j.StockLotId,
                LotNumber = j.StockLot?.LotNumber,
                Shifts = _mapper.Map<List<ShiftDto>>(j.Shifts)
            };
        }).ToList();
    }
}
