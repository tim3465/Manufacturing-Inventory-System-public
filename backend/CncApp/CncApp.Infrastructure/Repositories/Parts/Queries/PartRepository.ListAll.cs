using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class PartRepository
{
    public async Task<List<Part>> ListAllAsync(CancellationToken ct = default)
    {
        return await _context.Parts.ToListAsync(ct);
    }
}

