using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class PartRepository
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var part = await _context.Parts.FindAsync(new object[] { id }, ct);
        if (part == null)
            return false;

        part.Inactivate(inactivatedByUserId);

        return true;
    }
}

