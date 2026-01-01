using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class MachineRepository : IMachineRepository
{
    public async Task<List<Machine>> ListActiveAsync(CancellationToken ct = default)
    {
        return await _context.Machines.Where(m =>!m.InactivatedDateTime.HasValue).ToListAsync(ct);
    }
}

