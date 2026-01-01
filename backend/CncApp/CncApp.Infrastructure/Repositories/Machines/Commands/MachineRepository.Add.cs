using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class MachineRepository : IMachineRepository
{
    public async Task AddAsync(Machine machine, CancellationToken ct = default)
    {
        await _context.Machines.AddAsync(machine, ct);
    }
}

