using CncApp.Domain.Entities;

namespace CncApp.Infrastructure.Repositories;

public partial class MaterialRepository
{
    public async Task AddAsync(Material material, CancellationToken ct = default)
    {
        await _context.Materials.AddAsync(material, ct);
    }
}

