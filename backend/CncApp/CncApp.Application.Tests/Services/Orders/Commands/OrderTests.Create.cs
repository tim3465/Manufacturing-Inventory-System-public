using AutoMapper;
using CncApp.Application.Dtos.Orders;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Orders;

public partial class OrderTests
{
    [Fact]
    public async Task CreateAsync_WhenValidDto_CreatesOrderAndReturnsId()
    {
        // Arrange
        var dto = new CreateOrderRequestDto
        {
            PartId = 1,
            CustomerId = 1,
            PartAmountRequested = 100,
            PartsPerBar = 10
        };
        var cancellationToken = CancellationToken.None;

        var order = new Order(dto.PartId, dto.CustomerId, dto.PartAmountRequested, dto.PartsPerBar)
        {
            Id = 1
        };

        MockMapper
            .Setup(m => m.Map<Order>(dto))
            .Returns(order);

        MockRepository
            .Setup(r => r.AddAsync(It.IsAny<Order>(), cancellationToken))
            .Returns(Task.CompletedTask);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await OrderService.CreateAsync(dto, cancellationToken);

        // Assert
        Assert.Equal(1, result);

        MockMapper.Verify(m => m.Map<Order>(dto), Times.Once);
        MockRepository.Verify(r => r.AddAsync(It.IsAny<Order>(), cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }
}

