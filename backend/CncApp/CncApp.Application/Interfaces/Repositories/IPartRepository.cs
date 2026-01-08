using CncApp.Domain.Entities;

namespace CncApp.Application.Interfaces.Repositories;

public interface IPartRepository
{
    Task<Part?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<List<Part>> ListActiveAsync(CancellationToken ct = default);

    Task<List<Part>> ListAllAsync(CancellationToken ct = default);

    Task AddAsync(Part part, CancellationToken ct = default);

    Task UpdateAsync(Part part, CancellationToken ct = default);

    Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}

