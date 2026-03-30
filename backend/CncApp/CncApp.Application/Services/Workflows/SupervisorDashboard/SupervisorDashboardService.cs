using CncApp.Application.Interfaces.Repositories;

namespace CncApp.Application.Services.Workflows.SupervisorDashboard;

public partial class SupervisorDashboardService
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IOrderRepository _orderRepository;

    public SupervisorDashboardService(
        IShiftRepository shiftRepository,
        IJobRepository jobRepository,
        IOrderRepository orderRepository)
    {
        _shiftRepository = shiftRepository;
        _jobRepository = jobRepository;
        _orderRepository = orderRepository;
    }
}
