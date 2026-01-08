using CncApp.Application.Dtos.Jobs;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<int> CreateAsync(CreateJobRequestDto dto, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

