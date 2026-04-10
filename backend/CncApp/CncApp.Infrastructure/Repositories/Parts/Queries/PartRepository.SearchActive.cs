using CncApp.Application.Dtos.Parts;
using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class PartRepository
{
    public async Task<(List<Part> Items, int TotalCount)> SearchActiveAsync(
        PartSearchRequestDto request, CancellationToken ct = default)
    {
        var query = _context.Parts
            .Where(p => !p.InactivatedDateTime.HasValue);

        if (!string.IsNullOrWhiteSpace(request.PartName))
        {
            query = query.Where(p => p.PartName.Contains(request.PartName));
        }

        if (!string.IsNullOrWhiteSpace(request.PartNumber))
        {
            query = query.Where(p => p.PartNumber.Contains(request.PartNumber));
        }

        var isDescending = request.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        query = request.SortColumn switch
        {
            "PartNumber" => isDescending
                ? query.OrderByDescending(p => p.PartNumber)
                : query.OrderBy(p => p.PartNumber),
            _ => isDescending
                ? query.OrderByDescending(p => p.PartName)
                : query.OrderBy(p => p.PartName),
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
