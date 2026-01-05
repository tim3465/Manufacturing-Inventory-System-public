using AutoMapper;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.StockLots;
using Moq;

namespace CncApp.Application.Tests.Services.StockLots;

public partial class StockLotTests
{
    protected readonly Mock<IStockLotRepository> MockRepository;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly StockLotService StockLotService;

    public StockLotTests()
    {
        MockRepository = new Mock<IStockLotRepository>();
        MockMapper = new Mock<IMapper>();
        StockLotService = new StockLotService(MockRepository.Object, MockMapper.Object);
    }
}

