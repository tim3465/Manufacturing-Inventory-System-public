using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftRepository
{
    public async Task<(List<Shift> Items, int TotalCount)> SearchByOperatorAsync(
        int operatorId, ShiftLogSearchRequestDto request, CancellationToken ct = default)
    {
        var query = _context.Shifts
            .Where(s => s.OperatorId == operatorId
                     && !s.InactivatedDateTime.HasValue
                     && s.StopTime != null)
            .Include(s => s.Job)
                .ThenInclude(j => j.Machine)
            .Include(s => s.Job)
                .ThenInclude(j => j.Order)
                    .ThenInclude(o => o.Part)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.MachineName))
        {
            query = query.Where(s => s.Job.Machine.SerialNumber.Contains(request.MachineName));
        }

        if (!string.IsNullOrWhiteSpace(request.JobNumber))
        {
            query = query.Where(s => s.JobId.ToString().Contains(request.JobNumber));
        }

        if (!string.IsNullOrWhiteSpace(request.PartNumber))
        {
            query = query.Where(s => s.Job.Order.Part.PartNumber.Contains(request.PartNumber));
        }

        if (request.StartTimeFrom.HasValue)
        {
            query = query.Where(s => s.StartTime >= request.StartTimeFrom.Value);
        }

        if (request.StartTimeTo.HasValue)
        {
            query = query.Where(s => s.StartTime <= request.StartTimeTo.Value);
        }

        if (request.StopTimeFrom.HasValue)
        {
            query = query.Where(s => s.StopTime >= request.StopTimeFrom.Value);
        }

        if (request.StopTimeTo.HasValue)
        {
            query = query.Where(s => s.StopTime <= request.StopTimeTo.Value);
        }

        var totalCount = await query.CountAsync(ct);

        var isAscending = string.Equals(request.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);

        query = request.SortColumn switch
        {
            "StopTime" => isAscending
                ? query.OrderBy(s => s.StopTime)
                : query.OrderByDescending(s => s.StopTime),
            "MachineSerialNumber" => isAscending
                ? query.OrderBy(s => s.Job.Machine.SerialNumber)
                : query.OrderByDescending(s => s.Job.Machine.SerialNumber),
            "JobNumber" => isAscending
                ? query.OrderBy(s => s.JobId)
                : query.OrderByDescending(s => s.JobId),
            "PartNumber" => isAscending
                ? query.OrderBy(s => s.Job.Order.Part.PartNumber)
                : query.OrderByDescending(s => s.Job.Order.Part.PartNumber),
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
