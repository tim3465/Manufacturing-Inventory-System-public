using AutoMapper;
using CncApp.Application.Dtos.Orders;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Orders;

public partial class OrderTests
{
    [Fact]
    public async Task ListAllAsync_WhenOrdersExist_ReturnsListOfOrderDtos()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var orders = new List<Order>
        {
            new Order(1, 1, 100, 10) { Id = 1 },
            new Order(2, 2, 200, 20) { Id = 2 },
            new Order(3, 3, 300, 30) { Id = 3 }
        };

        var expectedDtos = new List<OrderDto>
        {
            new OrderDto { Id = 1, PartId = 1, CustomerId = 1, PartAmountRequested = 100, PartsPerBar = 10 },
            new OrderDto { Id = 2, PartId = 2, CustomerId = 2, PartAmountRequested = 200, PartsPerBar = 20 },
            new OrderDto { Id = 3, PartId = 3, CustomerId = 3, PartAmountRequested = 300, PartsPerBar = 30 }
        };

        MockRepository
            .Setup(r => r.ListAllAsync(cancellationToken))
            .ReturnsAsync(orders);

        MockMapper
            .Setup(m => m.Map<List<OrderDto>>(orders))
            .Returns(expectedDtos);

        // Act
        var result = await OrderService.ListAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);
        Assert.Equal(3, result[2].Id);

        MockRepository.Verify(r => r.ListAllAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<OrderDto>>(orders), Times.Once);
    }

    [Fact]
    public async Task ListAllAsync_WhenNoOrdersExist_ReturnsEmptyList()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var orders = new List<Order>();

        MockRepository
            .Setup(r => r.ListAllAsync(cancellationToken))
            .ReturnsAsync(orders);

        MockMapper
            .Setup(m => m.Map<List<OrderDto>>(orders))
            .Returns(new List<OrderDto>());

        // Act
        var result = await OrderService.ListAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        MockRepository.Verify(r => r.ListAllAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<OrderDto>>(orders), Times.Once);
    }
}

