using CncApp.Domain.Entities;

namespace CncApp.Application.Interfaces.Repositories;

public interface IShiftRepository
{
    Task<Shift?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Shift>> ListActiveAsync(CancellationToken ct = default);
    Task<List<Shift>> ListAllAsync(CancellationToken ct = default);
    Task AddAsync(Shift shift, CancellationToken ct = default);
    Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

