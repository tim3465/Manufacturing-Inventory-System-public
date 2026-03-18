using AutoMapper;
using CncApp.Application.Interfaces;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.StockLotAdjustments;
using CncApp.Application.Services.Workflows.StartJob;
using Moq;

namespace CncApp.Application.Tests.Services.Workflows.StartJob;

public partial class StartJobTests
{
    protected readonly Mock<IJobRepository> MockJobRepository;
    protected readonly Mock<IStockLotAdjustmentRepository> MockStockLotAdjustmentRepository;
    protected readonly Mock<IStockLotRepository> MockStockLotRepository;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly Mock<ITransactionManager> MockTransactionManager;
    protected readonly StartJobService Service;

    public StartJobTests()
    {
        MockJobRepository = new Mock<IJobRepository>();
        MockStockLotAdjustmentRepository = new Mock<IStockLotAdjustmentRepository>();
        MockStockLotRepository = new Mock<IStockLotRepository>();
        MockMapper = new Mock<IMapper>();
        MockTransactionManager = new Mock<ITransactionManager>();

        var stockLotAdjustmentService = new StockLotAdjustmentService(
            MockStockLotAdjustmentRepository.Object,
            MockStockLotRepository.Object,
            MockTransactionManager.Object,
            MockMapper.Object);

        Service = new StartJobService(
            MockJobRepository.Object,
            stockLotAdjustmentService,
            MockTransactionManager.Object);
    }
}
