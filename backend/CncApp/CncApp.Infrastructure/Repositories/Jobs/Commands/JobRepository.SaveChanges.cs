namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}

