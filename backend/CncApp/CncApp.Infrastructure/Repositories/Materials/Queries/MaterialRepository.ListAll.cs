using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class MaterialRepository
{
    public async Task<List<Material>> ListAllAsync(CancellationToken ct = default)
    {
        return await _context.Materials.ToListAsync(ct);
    }
}

