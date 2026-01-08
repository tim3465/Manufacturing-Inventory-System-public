using AutoMapper;
using CncApp.Application.Dtos.Orders;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Orders;

public partial class OrderTests
{
    [Fact]
    public async Task UpdateAsync_WhenOrderExists_UpdatesAndReturnsOrderDto()
    {
        // Arrange
        var orderId = 1;
        var dto = new UpdateOrderRequestDto
        {
            PartId = 2,
            CustomerId = 2,
            PartAmountRequested = 200,
            PartsPerBar = 20
        };
        var cancellationToken = CancellationToken.None;

        var order = new Order(1, 1, 100, 10)
        {
            Id = orderId
        };

        var expectedDto = new OrderDto
        {
            Id = orderId,
            PartId = 2,
            CustomerId = 2,
            PartAmountRequested = 200,
            PartsPerBar = 20
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(orderId, cancellationToken))
            .ReturnsAsync(order);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        MockMapper
            .Setup(m => m.Map<OrderDto>(It.IsAny<Order>()))
            .Returns(expectedDto);

        // Act
        var result = await OrderService.UpdateAsync(orderId, dto, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
        Assert.Equal(2, result.PartId);
        Assert.Equal(2, result.CustomerId);
        Assert.Equal(200, result.PartAmountRequested);
        Assert.Equal(20, result.PartsPerBar);

        Assert.Equal(2, order.PartId);
        Assert.Equal(2, order.CustomerId);
        Assert.Equal(200, order.PartAmountRequested);
        Assert.Equal(20, order.PartsPerBar);

        MockRepository.Verify(r => r.GetByIdAsync(orderId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<OrderDto>(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenOrderDoesNotExist_ReturnsNull()
    {
        // Arrange
        var orderId = 999;
        var dto = new UpdateOrderRequestDto
        {
            PartId = 2,
            CustomerId = 2,
            PartAmountRequested = 200,
            PartsPerBar = 20
        };
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(orderId, cancellationToken))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await OrderService.UpdateAsync(orderId, dto, cancellationToken);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(orderId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
        MockMapper.Verify(m => m.Map<OrderDto>(It.IsAny<Order>()), Times.Never);
    }
}

