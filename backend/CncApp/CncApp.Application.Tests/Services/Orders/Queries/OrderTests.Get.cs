using AutoMapper;
using CncApp.Application.Dtos.Orders;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Orders;

public partial class OrderTests
{
    [Fact]
    public async Task GetAsync_WhenOrderExists_ReturnsOrderDto()
    {
        // Arrange
        var orderId = 1;
        var cancellationToken = CancellationToken.None;

        var order = new Order(1, 1, 100, 10)
        {
            Id = orderId
        };

        var expectedDto = new OrderDto
        {
            Id = orderId,
            PartId = 1,
            CustomerId = 1,
            PartAmountRequested = 100,
            PartsPerBar = 10
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(orderId, cancellationToken))
            .ReturnsAsync(order);

        MockMapper
            .Setup(m => m.Map<OrderDto>(order))
            .Returns(expectedDto);

        // Act
        var result = await OrderService.GetAsync(orderId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
        Assert.Equal(1, result.PartId);
        Assert.Equal(1, result.CustomerId);
        Assert.Equal(100, result.PartAmountRequested);
        Assert.Equal(10, result.PartsPerBar);

        MockRepository.Verify(r => r.GetByIdAsync(orderId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<OrderDto>(order), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenOrderDoesNotExist_ReturnsNull()
    {
        // Arrange
        var orderId = 999;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(orderId, cancellationToken))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await OrderService.GetAsync(orderId, cancellationToken);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(orderId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<OrderDto>(It.IsAny<Order>()), Times.Never);
    }
}

