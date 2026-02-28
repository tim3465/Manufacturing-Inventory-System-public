using AutoMapper;
using CncApp.Application.Interfaces;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Materials;
using CncApp.Application.Services.StockLotAdjustments;
using CncApp.Application.Services.StockLots;
using CncApp.Application.Services.Workflows.ShippingReceiving;
using Moq;

namespace CncApp.Application.Tests.Services.Workflows.ShippingReceiving;

public partial class ShippingReceivingTests
{
    protected readonly Mock<IMaterialRepository> MockMaterialRepository;
    protected readonly Mock<IStockLotRepository> MockStockLotRepository;
    protected readonly Mock<IStockLotAdjustmentRepository> MockStockLotAdjustmentRepository;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly Mock<ITransactionManager> MockTransactionManager;
    protected readonly ShippingReceivingService Service;

    public ShippingReceivingTests()
    {
        MockMaterialRepository = new Mock<IMaterialRepository>();
        MockStockLotRepository = new Mock<IStockLotRepository>();
        MockStockLotAdjustmentRepository = new Mock<IStockLotAdjustmentRepository>();
        MockMapper = new Mock<IMapper>();
        MockTransactionManager = new Mock<ITransactionManager>();

        var materialService = new MaterialService(MockMaterialRepository.Object, MockMapper.Object);
        var stockLotService = new StockLotService(MockStockLotRepository.Object, MockMapper.Object);
        var stockLotAdjustmentService = new StockLotAdjustmentService(
            MockStockLotAdjustmentRepository.Object, MockMapper.Object);

        Service = new ShippingReceivingService(
            materialService,
            stockLotService,
            stockLotAdjustmentService,
            MockStockLotRepository.Object,
            MockTransactionManager.Object);
    }
}
