using CncApp.Application.Dtos.Jobs;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<List<JobShiftDto>?> GetJobShiftsForOperatorAsync(int jobId, int operatorId, CancellationToken ct = default)
    {
        var job = await _jobRepository.GetWithShiftsByIdForOperatorAsync(jobId, operatorId, ct);
        if (job == null) return null;
        return _mapper.Map<List<JobShiftDto>>(job.Shifts);
    }
}
