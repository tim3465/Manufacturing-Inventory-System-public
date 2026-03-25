using CncApp.Application.Dtos.StockLots;
using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class StockLotRepository
{
    public async Task<(List<StockLot> Items, int TotalCount)> SearchActiveAsync(
        StockLotSearchRequestDto request, CancellationToken ct = default)
    {
        var query = _context.StockLots
            .Where(sl => !sl.InactivatedDateTime.HasValue)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.LotNumber))
        {
            query = query.Where(sl => sl.LotNumber.Contains(request.LotNumber));
        }

        if (request.CheckedInFrom.HasValue)
        {
            var from = request.CheckedInFrom.Value.UtcDateTime;
            query = query.Where(sl => sl.CheckedInDateTime >= from);
        }

        if (request.CheckedInTo.HasValue)
        {
            var to = request.CheckedInTo.Value.UtcDateTime;
            query = query.Where(sl => sl.CheckedInDateTime <= to);
        }

        if (request.DiameterExact.HasValue)
        {
            query = query.Where(sl => sl.Diameter == request.DiameterExact.Value);
        }
        else
        {
            if (request.DiameterMin.HasValue)
            {
                query = query.Where(sl => sl.Diameter >= request.DiameterMin.Value);
            }

            if (request.DiameterMax.HasValue)
            {
                query = query.Where(sl => sl.Diameter <= request.DiameterMax.Value);
            }
        }

        var totalCount = await query.CountAsync(ct);

        var isAscending = string.Equals(request.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);

        query = request.SortColumn switch
        {
            "LotNumber" => isAscending
                ? query.OrderBy(sl => sl.LotNumber)
                : query.OrderByDescending(sl => sl.LotNumber),
            "AmountOfBars" => isAscending
                ? query.OrderBy(sl => sl.AmountOfBars)
                : query.OrderByDescending(sl => sl.AmountOfBars),
            "Diameter" => isAscending
                ? query.OrderBy(sl => sl.Diameter)
                : query.OrderByDescending(sl => sl.Diameter),
            "CheckedInDateTime" => isAscending
                ? query.OrderBy(sl => sl.CheckedInDateTime)
                : query.OrderByDescending(sl => sl.CheckedInDateTime),
            _ => query.OrderByDescending(sl => sl.CheckedInDateTime)
        };

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
