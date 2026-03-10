using CncApp.Application.Dtos.Jobs;
using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<List<JobProductionDto>> ListProductionAsync(CancellationToken ct = default)
    {
        var jobs = await _jobRepository.ListActiveWithShiftsAsync(ct);

        return jobs.Select(j => new JobProductionDto
        {
            Id = j.Id,
            OrderId = j.OrderId,
            DueDate = j.DueDate,
            MachineId = j.MachineId,
            PartAmountPlanned = j.PartAmountPlanned,
            Shifts = _mapper.Map<List<ShiftDto>>(j.Shifts)
        }).ToList();
    }
}
