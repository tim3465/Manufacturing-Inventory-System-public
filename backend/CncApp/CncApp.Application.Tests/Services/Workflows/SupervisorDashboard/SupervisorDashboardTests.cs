using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Workflows.SupervisorDashboard;
using CncApp.Domain.Entities;
using Moq;

namespace CncApp.Application.Tests.Services.Workflows.SupervisorDashboard;

public partial class SupervisorDashboardTests
{
    protected readonly Mock<IShiftRepository> MockShiftRepository;
    protected readonly Mock<IJobRepository> MockJobRepository;
    protected readonly Mock<IOrderRepository> MockOrderRepository;
    protected readonly SupervisorDashboardService Service;

    public SupervisorDashboardTests()
    {
        MockShiftRepository = new Mock<IShiftRepository>();
        MockJobRepository = new Mock<IJobRepository>();
        MockOrderRepository = new Mock<IOrderRepository>();

        MockOrderRepository
            .Setup(r => r.ListActiveWithDetailsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Order>());

        Service = new SupervisorDashboardService(
            MockShiftRepository.Object,
            MockJobRepository.Object,
            MockOrderRepository.Object);
    }
}
