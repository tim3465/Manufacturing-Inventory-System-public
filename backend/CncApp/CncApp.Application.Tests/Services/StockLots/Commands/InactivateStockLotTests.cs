using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.StockLots;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.StockLots.Commands;

public class InactivateStockLotTests
{
    private readonly Mock<IStockLotRepository> _mockRepository;
    private readonly Mock<AutoMapper.IMapper> _mockMapper;
    private readonly StockLotService _stockLotService;

    public InactivateStockLotTests()
    {
        _mockRepository = new Mock<IStockLotRepository>();
        _mockMapper = new Mock<AutoMapper.IMapper>();
        _stockLotService = new StockLotService(_mockRepository.Object, _mockMapper.Object);
    }

    // TODO: Add test methods
}

