using CncApp.Application.Dtos.Orders;
using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class OrderRepository
{
    public async Task<(List<Order> Items, int TotalCount)> SearchActiveWithDetailsAsync(
        OrderProductionSearchRequestDto request, CancellationToken ct = default)
    {
        var query = _context.Orders
            .Where(o => !o.InactivatedDateTime.HasValue)
            .Include(o => o.Customer)
            .Include(o => o.Part)
            .Include(o => o.Jobs)
                .ThenInclude(j => j.Shifts)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.CustomerName))
            query = query.Where(o => o.Customer != null && o.Customer.CompanyName.Contains(request.CustomerName));

        if (!string.IsNullOrWhiteSpace(request.PartName))
            query = query.Where(o => o.Part != null && o.Part.PartName.Contains(request.PartName));

        if (!string.IsNullOrWhiteSpace(request.PartNumber))
            query = query.Where(o => o.Part != null && o.Part.PartNumber.Contains(request.PartNumber));

        var isAsc = request.SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);

        query = request.SortColumn.ToLowerInvariant() switch
        {
            "partname"   => isAsc ? query.OrderBy(o => o.Part!.PartName)   : query.OrderByDescending(o => o.Part!.PartName),
            "partnumber" => isAsc ? query.OrderBy(o => o.Part!.PartNumber) : query.OrderByDescending(o => o.Part!.PartNumber),
            _            => isAsc ? query.OrderBy(o => o.Customer!.CompanyName) : query.OrderByDescending(o => o.Customer!.CompanyName),
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
