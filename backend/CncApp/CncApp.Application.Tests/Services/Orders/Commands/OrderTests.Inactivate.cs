using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Orders;

public partial class OrderTests
{
    [Fact]
    public async Task InactivateAsync_WhenOrderExists_ReturnsTrue()
    {
        // Arrange
        var orderId = 1;
        var inactivatedByUserId = 5;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.InactivateAsync(orderId, inactivatedByUserId, cancellationToken))
            .ReturnsAsync(true);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await OrderService.InactivateAsync(orderId, inactivatedByUserId, cancellationToken);

        // Assert
        Assert.True(result);

        MockRepository.Verify(r => r.InactivateAsync(orderId, inactivatedByUserId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InactivateAsync_WhenOrderDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var orderId = 999;
        int? inactivatedByUserId = null;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.InactivateAsync(orderId, inactivatedByUserId, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await OrderService.InactivateAsync(orderId, inactivatedByUserId, cancellationToken);

        // Assert
        Assert.False(result);

        MockRepository.Verify(r => r.InactivateAsync(orderId, inactivatedByUserId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
    }
}

