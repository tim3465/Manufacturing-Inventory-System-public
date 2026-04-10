using CncApp.Application.Dtos.Jobs;
using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository
{
    public async Task<(List<Job> Items, int TotalCount)> SearchProductionAsync(
        JobProductionSearchRequestDto request, CancellationToken ct = default)
    {
        var query = _context.Jobs
            .Where(j => !j.InactivatedDateTime.HasValue)
            .Include(j => j.Machine)
            .Include(j => j.Order)
                .ThenInclude(o => o.Part)
            .Include(j => j.StockLot)
            .Include(j => j.Shifts)
            .AsQueryable();

        if (request.DueDateFrom.HasValue)
            query = query.Where(j => j.DueDate >= request.DueDateFrom.Value);

        if (request.DueDateTo.HasValue)
            query = query.Where(j => j.DueDate <= request.DueDateTo.Value);

        if (!string.IsNullOrWhiteSpace(request.OrderNumber))
            query = query.Where(j => j.OrderId.ToString().Contains(request.OrderNumber));

        if (!string.IsNullOrWhiteSpace(request.PartName))
            query = query.Where(j => j.Order.Part.PartName.Contains(request.PartName));

        if (!string.IsNullOrWhiteSpace(request.PartNumber))
            query = query.Where(j => j.Order.Part.PartNumber.Contains(request.PartNumber));

        if (!string.IsNullOrWhiteSpace(request.MachineName))
            query = query.Where(j => j.Machine.SerialNumber.Contains(request.MachineName));

        if (!string.IsNullOrWhiteSpace(request.LotNumber))
            query = query.Where(j => j.StockLot != null && j.StockLot.LotNumber.Contains(request.LotNumber));

        var totalCount = await query.CountAsync(ct);

        var isAscending = string.Equals(request.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);

        query = request.SortColumn switch
        {
            "OrderId" => isAscending
                ? query.OrderBy(j => j.OrderId)
                : query.OrderByDescending(j => j.OrderId),
            "PartName" => isAscending
                ? query.OrderBy(j => j.Order.Part.PartName)
                : query.OrderByDescending(j => j.Order.Part.PartName),
            "PartNumber" => isAscending
                ? query.OrderBy(j => j.Order.Part.PartNumber)
                : query.OrderByDescending(j => j.Order.Part.PartNumber),
            "MachineName" => isAscending
                ? query.OrderBy(j => j.Machine.SerialNumber)
                : query.OrderByDescending(j => j.Machine.SerialNumber),
            "LotNumber" => isAscending
                ? query.OrderBy(j => j.StockLot!.LotNumber)
                : query.OrderByDescending(j => j.StockLot!.LotNumber),
            _ => isAscending
                ? query.OrderBy(j => j.DueDate)
                : query.OrderByDescending(j => j.DueDate)
        };

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
