using AutoMapper;
using CncApp.Application.Interfaces;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.StockLotAdjustments;
using Moq;

namespace CncApp.Application.Tests.Services.StockLotAdjustments;

public partial class StockLotAdjustmentTests
{
    protected readonly Mock<IStockLotAdjustmentRepository> MockRepository;
    protected readonly Mock<IStockLotRepository> MockStockLotRepository;
    protected readonly Mock<ITransactionManager> MockTransactionManager;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly StockLotAdjustmentService StockLotAdjustmentService;

    public StockLotAdjustmentTests()
    {
        MockRepository = new Mock<IStockLotAdjustmentRepository>();
        MockStockLotRepository = new Mock<IStockLotRepository>();
        MockTransactionManager = new Mock<ITransactionManager>();
        MockMapper = new Mock<IMapper>();
        StockLotAdjustmentService = new StockLotAdjustmentService(
            MockRepository.Object,
            MockStockLotRepository.Object,
            MockTransactionManager.Object,
            MockMapper.Object);
    }
}
