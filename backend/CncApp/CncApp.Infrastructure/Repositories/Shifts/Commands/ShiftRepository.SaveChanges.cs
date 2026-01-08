namespace CncApp.Infrastructure.Repositories;

public partial class ShiftRepository
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}


