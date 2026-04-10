using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftRepository
{
    public async Task<(List<Shift> Items, int TotalCount)> SearchProductionAsync(
        ShiftProductionSearchRequestDto request, CancellationToken ct = default)
    {
        var query = _context.Shifts
            .Where(s => !s.InactivatedDateTime.HasValue)
            .Include(s => s.Operator)
            .Include(s => s.Job)
                .ThenInclude(j => j.Order)
                    .ThenInclude(o => o.Part)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.OperatorName))
            query = query.Where(s => (s.Operator.FirstName + " " + s.Operator.LastName).Contains(request.OperatorName));

        if (!string.IsNullOrWhiteSpace(request.JobNumber))
            query = query.Where(s => s.JobId.ToString().Contains(request.JobNumber));

        if (request.StartTimeFrom.HasValue)
            query = query.Where(s => s.StartTime >= request.StartTimeFrom.Value);

        if (request.StartTimeTo.HasValue)
            query = query.Where(s => s.StartTime <= request.StartTimeTo.Value);

        if (request.StopTimeFrom.HasValue)
            query = query.Where(s => s.StopTime >= request.StopTimeFrom.Value);

        if (request.StopTimeTo.HasValue)
            query = query.Where(s => s.StopTime <= request.StopTimeTo.Value);

        var totalCount = await query.CountAsync(ct);

        var isAscending = string.Equals(request.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);

        query = request.SortColumn switch
        {
            "OperatorName" => isAscending
                ? query.OrderBy(s => (s.Operator.FirstName + " " + s.Operator.LastName))
                : query.OrderByDescending(s => (s.Operator.FirstName + " " + s.Operator.LastName)),
            "JobNumber" => isAscending
                ? query.OrderBy(s => s.JobId)
                : query.OrderByDescending(s => s.JobId),
            "StopTime" => isAscending
                ? query.OrderBy(s => s.StopTime)
                : query.OrderByDescending(s => s.StopTime),
            _ => isAscending
                ? query.OrderBy(s => s.StartTime)
                : query.OrderByDescending(s => s.StartTime)
        };

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
