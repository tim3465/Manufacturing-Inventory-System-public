using CncApp.Application.Dtos.SupervisorDashboard;

namespace CncApp.Application.Services.Workflows.SupervisorDashboard;

public partial class SupervisorDashboardService
{
    public async Task<SupervisorDashboardDto> GetDashboardAsync(CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var openShifts = await _shiftRepository.ListOpenWithContextAsync(ct);
        var todayShifts = await _shiftRepository.ListStartedTodayAsync(today, ct);
        var lateJobs = await _jobRepository.ListLateAsync(today, ct);

        var machinesRunning = openShifts.Select(s => s.Job.MachineId).Distinct().Count();
        var operatorsActive = openShifts.Select(s => s.OperatorId).Distinct().Count();

        var operatorGroups = openShifts.GroupBy(s => s.OperatorId);

        var operators = operatorGroups.Select(group =>
        {
            var operatorId = group.Key;
            var firstShift = group.First();
            var op = firstShift.Operator;
            var operatorName = $"{op.FirstName ?? string.Empty} {op.LastName ?? string.Empty}".Trim();

            var activeJobs = group.Select(s => new SupervisorDashboardActiveJobDto
            {
                JobId = s.JobId,
                PartName = s.Job.Order.Part.PartName,
                MachineName = s.Job.Machine.SerialNumber
            }).ToList();

            var todayShiftsForOperator = todayShifts.Where(s => s.OperatorId == operatorId).ToList();
            var partsMadeToday = todayShiftsForOperator.Sum(s => s.PartsMade);
            var scrapToday = todayShiftsForOperator.Sum(s => s.Scrap);
            var total = partsMadeToday + scrapToday;
            var scrapPercentage = total > 0 ? Math.Round(scrapToday / (decimal)total * 100, 1) : 0;

            return new SupervisorDashboardOperatorDto
            {
                OperatorId = operatorId,
                OperatorName = operatorName,
                MachinesRunning = group.Select(s => s.Job.MachineId).Distinct().Count(),
                ActiveJobs = activeJobs,
                PartsMadeToday = partsMadeToday,
                ScrapToday = scrapToday,
                ScrapPercentage = scrapPercentage
            };
        }).ToList();

        return new SupervisorDashboardDto
        {
            MachinesRunning = machinesRunning,
            OperatorsActive = operatorsActive,
            LateJobs = lateJobs.Count,
            Operators = operators
        };
    }
}
