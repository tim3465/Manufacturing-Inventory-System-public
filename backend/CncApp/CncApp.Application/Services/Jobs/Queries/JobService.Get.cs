using CncApp.Application.Dtos.Jobs;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<JobDto?> GetAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

