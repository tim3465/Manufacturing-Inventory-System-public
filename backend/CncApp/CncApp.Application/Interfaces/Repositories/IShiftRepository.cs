using CncApp.Domain.Entities;

namespace CncApp.Application.Interfaces.Repositories;

public interface IShiftRepository
{
    Task<Shift?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Shift>> ListByJobIdAsync(int jobId, CancellationToken ct = default);
    Task AddAsync(Shift shift, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

