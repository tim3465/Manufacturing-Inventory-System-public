using CncApp.Application.Dtos.Jobs;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<List<JobDto>> ListAllAsync(CancellationToken ct = default)
    {
        var jobs = await _jobRepository.ListAllAsync(ct);
        return _mapper.Map<List<JobDto>>(jobs);
    }
}

