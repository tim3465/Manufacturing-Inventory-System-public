using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class PartRepository
{
    public async Task<Part?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Parts.FindAsync(new object[] { id }, ct);
    }
}

