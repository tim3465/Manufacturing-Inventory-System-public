namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var result = await _jobRepository.InactivateAsync(id, inactivatedByUserId, ct);
        if (result)
        {
            await _jobRepository.SaveChangesAsync(ct);
        }

        return result;
    }
}

