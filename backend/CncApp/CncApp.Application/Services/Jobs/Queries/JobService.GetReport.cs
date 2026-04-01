using CncApp.Application.Dtos.Jobs;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<JobReportDto?> GetReportAsync(int id, CancellationToken ct = default)
    {
        var job = await _jobRepository.GetByIdWithShiftsAsync(id, ct);
        if (job == null)
            return null;

        var activeShifts = job.Shifts.ToList();

        var totalPartsMade = activeShifts.Sum(s => s.PartsMade);
        var totalScrap = activeShifts.Sum(s => s.Scrap);
        var totalBarsConsumed = activeShifts.Sum(s => s.BarsConsumed);
        var totalDowntime = activeShifts
            .Where(s => s.Downtime.HasValue)
            .Aggregate(TimeSpan.Zero, (acc, s) => acc + s.Downtime!.Value);

        var actualPartsPerBar = totalBarsConsumed > 0
            ? Math.Round((decimal)totalPartsMade / totalBarsConsumed, 2)
            : (decimal?)null;

        var jobStatus = job.EndedDateTime.HasValue
            ? "Completed"
            : job.StartedDateTime.HasValue
                ? "In Progress"
                : "Planned";

        return new JobReportDto
        {
            Id = job.Id,
            OrderId = job.OrderId,
            MachineName = job.Machine?.SerialNumber ?? string.Empty,
            PartName = job.Order?.Part?.PartName ?? string.Empty,
            PartNumber = job.Order?.Part?.PartNumber ?? string.Empty,
            DueDate = job.DueDate,
            StartedDateTime = job.StartedDateTime,
            EndedDateTime = job.EndedDateTime,
            JobStatus = jobStatus,
            PartAmountPlanned = job.PartAmountPlanned,
            TotalPartsMade = totalPartsMade,
            TotalScrap = totalScrap,
            BarAmountPlanned = job.BarAmountPlanned,
            TotalBarsConsumed = totalBarsConsumed,
            EstimatedPartsPerBar = job.EstimatedPartsPerBar,
            ActualPartsPerBar = actualPartsPerBar,
            TotalDowntime = totalDowntime,
            Shifts = activeShifts
                .OrderByDescending(s => s.StartTime)
                .Select(s => new JobReportShiftDto
                {
                    Id = s.Id,
                    OperatorName = s.Operator != null
                        ? (s.Operator.FirstName != null && s.Operator.LastName != null
                            ? $"{s.Operator.FirstName} {s.Operator.LastName}"
                            : s.Operator.UserName)
                        : string.Empty,
                    StartTime = s.StartTime,
                    StopTime = s.StopTime,
                    PartsMade = s.PartsMade,
                    Scrap = s.Scrap,
                    BarsConsumed = s.BarsConsumed,
                    PartsPerBar = s.PartsPerBar,
                    Downtime = s.Downtime
                })
                .ToList(),
            IssueLogs = activeShifts
                .SelectMany(s => s.ShiftIssueLogs.Select(log => new JobReportIssueLogDto
                {
                    Id = log.Id,
                    ShiftId = s.Id,
                    OperatorName = s.Operator != null
                        ? (s.Operator.FirstName != null && s.Operator.LastName != null
                            ? $"{s.Operator.FirstName} {s.Operator.LastName}"
                            : s.Operator.UserName)
                        : string.Empty,
                    CreatedDateTime = log.CreatedDateTime,
                    IssueType = log.IssueType,
                    Description = log.Description,
                    ScrapQuantity = log.ScrapQuantity,
                    Downtime = log.Downtime
                }))
                .OrderBy(l => l.CreatedDateTime)
                .ToList()
        };
    }
}
