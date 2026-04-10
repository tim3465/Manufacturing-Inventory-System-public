using CncApp.Application.Dtos.Jobs;
using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository
{
    public async Task<(List<Job> Items, int TotalCount)> SearchByOperatorAsync(
        int operatorId, MyJobSearchRequestDto request, CancellationToken ct = default)
    {
        var query = _context.Jobs
            .Where(j => !j.InactivatedDateTime.HasValue
                     && j.Shifts.Any(s => s.OperatorId == operatorId && !s.InactivatedDateTime.HasValue))
            .Include(j => j.Machine)
            .Include(j => j.Order)
                .ThenInclude(o => o.Part)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.JobNumber))
        {
            query = query.Where(j => j.Id.ToString().Contains(request.JobNumber));
        }

        if (!string.IsNullOrWhiteSpace(request.PartNumber))
        {
            query = query.Where(j => j.Order.Part.PartNumber.Contains(request.PartNumber));
        }

        if (!string.IsNullOrWhiteSpace(request.MachineName))
        {
            query = query.Where(j => j.Machine.SerialNumber.Contains(request.MachineName));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (string.Equals(request.Status, "In Progress", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(j => j.EndedDateTime == null);
            }
            else if (string.Equals(request.Status, "Completed", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(j => j.EndedDateTime != null);
            }
        }

        var totalCount = await query.CountAsync(ct);

        var isAscending = string.Equals(request.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);

        query = request.SortColumn switch
        {
            "PartNumber" => isAscending
                ? query.OrderBy(j => j.Order.Part.PartNumber)
                : query.OrderByDescending(j => j.Order.Part.PartNumber),
            "MachineName" => isAscending
                ? query.OrderBy(j => j.Machine.SerialNumber)
                : query.OrderByDescending(j => j.Machine.SerialNumber),
            _ => isAscending
                ? query.OrderBy(j => j.Id)
                : query.OrderByDescending(j => j.Id)
        };

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
