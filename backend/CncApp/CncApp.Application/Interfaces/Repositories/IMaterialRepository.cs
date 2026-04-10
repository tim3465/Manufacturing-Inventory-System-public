using CncApp.Application.Dtos.Materials;
using CncApp.Domain.Entities;

namespace CncApp.Application.Interfaces.Repositories;

public interface IMaterialRepository
{
    Task<Material?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Material>> ListActiveAsync(CancellationToken ct = default);
    Task<List<Material>> ListAllAsync(CancellationToken ct = default);
    Task<(List<Material> Items, int TotalCount)> SearchActiveAsync(MaterialSearchRequestDto request, CancellationToken ct = default);
    Task AddAsync(Material material, CancellationToken ct = default);
    Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

