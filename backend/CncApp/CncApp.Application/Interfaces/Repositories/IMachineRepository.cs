using CncApp.Domain.Entities;

namespace CncApp.Application.Interfaces.Repositories;

public interface IMachineRepository
{
    Task<Machine?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Machine>> ListAsync(CancellationToken ct = default);
    Task AddAsync(Machine machine, CancellationToken ct = default);
    Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}


