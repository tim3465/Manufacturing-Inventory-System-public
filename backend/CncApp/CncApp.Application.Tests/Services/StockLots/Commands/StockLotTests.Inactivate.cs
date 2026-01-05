using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.StockLots;

public partial class StockLotTests
{
    [Fact]
    public async Task InactivateAsync_WhenStockLotExists_InactivatesAndReturnsTrue()
    {
        // Arrange
        var stockLotId = 1;
        var userId = 1;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.InactivateAsync(stockLotId, userId, cancellationToken))
            .ReturnsAsync(true);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await StockLotService.InactivateAsync(stockLotId, userId, cancellationToken);

        // Assert
        Assert.True(result);

        MockRepository.Verify(r => r.InactivateAsync(stockLotId, userId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InactivateAsync_WhenStockLotDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var stockLotId = 999;
        var userId = 1;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.InactivateAsync(stockLotId, userId, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await StockLotService.InactivateAsync(stockLotId, userId, cancellationToken);

        // Assert
        Assert.False(result);

        MockRepository.Verify(r => r.InactivateAsync(stockLotId, userId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

