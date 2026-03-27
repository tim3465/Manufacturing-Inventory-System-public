using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Workflows.SupervisorDashboard;
using Moq;

namespace CncApp.Application.Tests.Services.Workflows.SupervisorDashboard;

public partial class SupervisorDashboardTests
{
    protected readonly Mock<IShiftRepository> MockShiftRepository;
    protected readonly Mock<IJobRepository> MockJobRepository;
    protected readonly SupervisorDashboardService Service;

    public SupervisorDashboardTests()
    {
        MockShiftRepository = new Mock<IShiftRepository>();
        MockJobRepository = new Mock<IJobRepository>();

        Service = new SupervisorDashboardService(
            MockShiftRepository.Object,
            MockJobRepository.Object);
    }
}
