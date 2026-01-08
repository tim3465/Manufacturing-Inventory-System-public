using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class PartRepository
{
    public async Task AddAsync(Part part, CancellationToken ct = default)
    {
        await _context.Parts.AddAsync(part, ct);
    }
}

