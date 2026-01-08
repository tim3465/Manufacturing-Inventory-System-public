using CncApp.Domain.Entities;

namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var job = await _context.Jobs.FindAsync(new object[] { id }, ct);
        if (job == null)
        {
            return false;
        }

        job.Inactivate(inactivatedByUserId);

        return true;
    }
}

