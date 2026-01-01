using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public class MachineRepository : IMachineRepository
{
    private readonly AppDbContext _context;

    public MachineRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Machine?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Machines.FindAsync(new object[] { id }, ct);
    }

    public async Task<List<Machine>> ListActiveAsync(CancellationToken ct = default)
    {
        return await _context.Machines.Where(m =>!m.InactivatedDateTime.HasValue).ToListAsync(ct);
    }

    public async Task<List<Machine>> ListAllAsync(CancellationToken ct = default)
    {
        return await _context.Machines.ToListAsync(ct);
    }


    public async Task AddAsync(Machine machine, CancellationToken ct = default)
    {
        await _context.Machines.AddAsync(machine, ct);
    }

    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var machine = await _context.Machines.FindAsync(new object[] { id }, ct);
        if (machine == null)
            return false;

        machine.InactivatedDateTime = DateTimeOffset.UtcNow;
        machine.InactivatedByUserId = inactivatedByUserId;

        return true;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}


