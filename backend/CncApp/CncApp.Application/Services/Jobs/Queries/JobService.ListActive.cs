using CncApp.Application.Dtos.Jobs;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<List<JobDto>> ListActiveAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

