using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.StockLotAdjustments;

public partial class StockLotAdjustmentTests
{
    [Fact]
    public async Task InactivateAsync_WhenAdjustmentExists_ReturnsTrue()
    {
        // Arrange
        var adjustmentId = 1;
        var inactivatedByUserId = 5;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.InactivateAsync(adjustmentId, inactivatedByUserId, cancellationToken))
            .ReturnsAsync(true);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await StockLotAdjustmentService.InactivateAsync(adjustmentId, inactivatedByUserId, cancellationToken);

        // Assert
        Assert.True(result);

        MockRepository.Verify(r => r.InactivateAsync(adjustmentId, inactivatedByUserId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InactivateAsync_WhenAdjustmentDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var adjustmentId = 999;
        int? inactivatedByUserId = null;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.InactivateAsync(adjustmentId, inactivatedByUserId, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await StockLotAdjustmentService.InactivateAsync(adjustmentId, inactivatedByUserId, cancellationToken);

        // Assert
        Assert.False(result);

        MockRepository.Verify(r => r.InactivateAsync(adjustmentId, inactivatedByUserId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
    }
}

