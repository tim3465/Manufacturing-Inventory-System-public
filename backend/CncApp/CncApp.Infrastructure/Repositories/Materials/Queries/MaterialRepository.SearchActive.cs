using CncApp.Application.Dtos.Materials;
using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class MaterialRepository
{
    public async Task<(List<Material> Items, int TotalCount)> SearchActiveAsync(
        MaterialSearchRequestDto request, CancellationToken ct = default)
    {
        var query = _context.Materials.Where(m => !m.InactivatedDateTime.HasValue);

        if (!string.IsNullOrWhiteSpace(request.HeatNumber))
            query = query.Where(m => m.HeatNumber.Contains(request.HeatNumber));

        if (!string.IsNullOrWhiteSpace(request.MaterialName))
            query = query.Where(m => m.MaterialName.Contains(request.MaterialName));

        query = request.SortColumn.ToLower() switch
        {
            "materialname" => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(m => m.MaterialName)
                : query.OrderBy(m => m.MaterialName),
            _ => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(m => m.HeatNumber)
                : query.OrderBy(m => m.HeatNumber)
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
