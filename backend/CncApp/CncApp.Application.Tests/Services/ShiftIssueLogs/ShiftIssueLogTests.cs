using AutoMapper;
using CncApp.Application.Interfaces;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.ShiftIssueLogs;
using Moq;

namespace CncApp.Application.Tests.Services.ShiftIssueLogs;

public partial class ShiftIssueLogTests
{
    protected readonly Mock<IShiftIssueLogRepository> MockRepository;
    protected readonly Mock<IShiftRepository> MockShiftRepository;
    protected readonly Mock<IUserRepository> MockUserRepository;
    protected readonly Mock<ITransactionManager> MockTransactionManager;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly ShiftIssueLogService ShiftIssueLogService;

    public ShiftIssueLogTests()
    {
        MockRepository = new Mock<IShiftIssueLogRepository>();
        MockShiftRepository = new Mock<IShiftRepository>();
        MockUserRepository = new Mock<IUserRepository>();
        MockTransactionManager = new Mock<ITransactionManager>();
        MockMapper = new Mock<IMapper>();
        ShiftIssueLogService = new ShiftIssueLogService(
            MockRepository.Object,
            MockShiftRepository.Object,
            MockUserRepository.Object,
            MockTransactionManager.Object,
            MockMapper.Object);
    }
}
