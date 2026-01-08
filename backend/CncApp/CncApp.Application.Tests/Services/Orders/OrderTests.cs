using AutoMapper;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Orders;
using Moq;

namespace CncApp.Application.Tests.Services.Orders;

public partial class OrderTests
{
    protected readonly Mock<IOrderRepository> MockRepository;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly OrderService OrderService;

    public OrderTests()
    {
        MockRepository = new Mock<IOrderRepository>();
        MockMapper = new Mock<IMapper>();
        OrderService = new OrderService(MockRepository.Object, MockMapper.Object);
    }
}

