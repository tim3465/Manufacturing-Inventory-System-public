using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Orders;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Orders.Commands;

public class CreateOrderTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<AutoMapper.IMapper> _mockMapper;
    private readonly OrderService _orderService;

    public CreateOrderTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockMapper = new Mock<AutoMapper.IMapper>();
        _orderService = new OrderService(_mockRepository.Object, _mockMapper.Object);
    }

    // TODO: Add test methods
}

