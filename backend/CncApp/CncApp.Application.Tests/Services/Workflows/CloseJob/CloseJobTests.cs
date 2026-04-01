using AutoMapper;
using CncApp.Application.Interfaces;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Shifts;
using CncApp.Application.Services.Workflows.CloseJob;
using Moq;

namespace CncApp.Application.Tests.Services.Workflows.CloseJob;

public partial class CloseJobTests
{
    protected readonly Mock<IShiftRepository> MockShiftRepository;
    protected readonly Mock<IJobRepository> MockJobRepository;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly Mock<ITransactionManager> MockTransactionManager;
    protected readonly CloseJobService Service;

    public CloseJobTests()
    {
        MockShiftRepository = new Mock<IShiftRepository>();
        MockJobRepository = new Mock<IJobRepository>();
        MockMapper = new Mock<IMapper>();
        MockTransactionManager = new Mock<ITransactionManager>();

        var shiftService = new ShiftService(MockShiftRepository.Object, MockJobRepository.Object, MockMapper.Object);

        Service = new CloseJobService(
            shiftService,
            MockJobRepository.Object,
            MockTransactionManager.Object);
    }
}
