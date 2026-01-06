using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class MaterialRepository
{
    public async Task<List<Material>> ListActiveAsync(CancellationToken ct = default)
    {
        return await _context.Materials.Where(m => !m.InactivatedDateTime.HasValue).ToListAsync(ct);
    }
}

