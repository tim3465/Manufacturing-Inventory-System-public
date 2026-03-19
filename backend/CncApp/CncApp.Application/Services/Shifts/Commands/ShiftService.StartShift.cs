using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public async Task<int> StartShiftAsync(StartShiftRequestDto dto, int operatorId, CancellationToken ct = default)
    {
        var job = await _jobRepository.GetByIdAsync(dto.JobId, ct);
        if (job == null || !job.StartedDateTime.HasValue)
            throw new InvalidOperationException("Job not found or not active.");

        var existing = await _shiftRepository.GetRunningShiftForMachineAsync(job.MachineId, ct);
        if (existing != null)
            throw new InvalidOperationException("Machine already has a running shift.");

        var shift = new Shift(
            jobId: dto.JobId,
            operatorId: operatorId,
            barsConsumed: 0,
            startTime: dto.StartTime);

        await _shiftRepository.AddAsync(shift, ct);
        await _shiftRepository.SaveChangesAsync(ct);

        return shift.Id;
    }
}
