using CncApp.Application.Dtos.Jobs;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<int> CreateAsync(CreateJobRequestDto dto, CancellationToken ct = default)
    {
        var job = _mapper.Map<Job>(dto);

        await _jobRepository.AddAsync(job, ct);
        await _jobRepository.SaveChangesAsync(ct);

        return job.Id;
    }
}

