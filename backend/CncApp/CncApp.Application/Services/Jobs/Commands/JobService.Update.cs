using CncApp.Application.Dtos.Jobs;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<JobDto?> UpdateAsync(int id, UpdateJobRequestDto dto, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

