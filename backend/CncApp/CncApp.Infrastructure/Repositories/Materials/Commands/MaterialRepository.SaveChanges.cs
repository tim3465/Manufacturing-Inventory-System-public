namespace CncApp.Infrastructure.Repositories;

public partial class MaterialRepository
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}

