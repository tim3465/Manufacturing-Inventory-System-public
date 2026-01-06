using CncApp.Domain.Entities;

namespace CncApp.Infrastructure.Repositories;

public partial class MaterialRepository
{
    public async Task<Material?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Materials.FindAsync(new object[] { id }, ct);
    }
}

