using CncApp.Application.Dtos.Jobs;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<List<JobDto>> ListActiveAsync(CancellationToken ct = default)
    {
        var jobs = await _jobRepository.ListActiveAsync(ct);
        return _mapper.Map<List<JobDto>>(jobs);
    }
}

