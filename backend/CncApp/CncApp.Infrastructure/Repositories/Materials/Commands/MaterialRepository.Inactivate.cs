using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class MaterialRepository
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var material = await _context.Materials.FindAsync(new object[] { id }, ct);
        if (material == null)
            return false;

        material.Inactivate(inactivatedByUserId);

        return true;
    }
}

