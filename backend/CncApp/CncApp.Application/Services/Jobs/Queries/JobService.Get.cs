using CncApp.Application.Dtos.Jobs;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<JobDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var job = await _jobRepository.GetByIdAsync(id, ct);
        if (job == null)
        {
            return null;
        }

        return _mapper.Map<JobDto>(job);
    }
}

