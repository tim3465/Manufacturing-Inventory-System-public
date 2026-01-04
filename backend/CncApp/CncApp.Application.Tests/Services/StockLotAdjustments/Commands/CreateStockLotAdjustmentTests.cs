using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.StockLotAdjustments;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.StockLotAdjustments.Commands;

public class CreateStockLotAdjustmentTests
{
    private readonly Mock<IStockLotAdjustmentRepository> _mockRepository;
    private readonly Mock<AutoMapper.IMapper> _mockMapper;
    private readonly StockLotAdjustmentService _stockLotAdjustmentService;

    public CreateStockLotAdjustmentTests()
    {
        _mockRepository = new Mock<IStockLotAdjustmentRepository>();
        _mockMapper = new Mock<AutoMapper.IMapper>();
        _stockLotAdjustmentService = new StockLotAdjustmentService(_mockRepository.Object, _mockMapper.Object);
    }

    // TODO: Add test methods
}

