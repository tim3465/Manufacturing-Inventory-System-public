using CncApp.Application.Interfaces.Repositories;

namespace CncApp.Application.Services.Workflows.SupervisorDashboard;

public partial class SupervisorDashboardService
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IJobRepository _jobRepository;

    public SupervisorDashboardService(IShiftRepository shiftRepository, IJobRepository jobRepository)
    {
        _shiftRepository = shiftRepository;
        _jobRepository = jobRepository;
    }
}
