using CncApp.Domain.Entities;
using CncApp.Domain.Enums;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.StockLotAdjustments;

public partial class StockLotAdjustmentTests
{
    // ── public InactivateAsync (controller path) ─────────────────────────

    [Fact]
    public async Task InactivateAsync_ReversesDeltaBarsAndCommitsTransaction()
    {
        // Arrange
        var adjustmentId = 1;
        var adjustment = new StockLotAdjustment(10, 8, StockLotAdjustmentReasonEnum.Received) { Id = adjustmentId };
        var stockLot = new StockLot("LOT-001", 1, 30, 25m, 3000m,
            StockLotConditionEnum.AsReceived, DateTime.UtcNow) { Id = 10 };

        MockRepository
            .Setup(r => r.GetByIdAsync(adjustmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(adjustment);
        MockRepository
            .Setup(r => r.InactivateAsync(adjustmentId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        MockRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockStockLotRepository
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stockLot);
        MockStockLotRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await StockLotAdjustmentService.InactivateAsync(adjustmentId);

        // Assert
        Assert.True(result);
        Assert.Equal(22, stockLot.AmountOfBars); // 30 - 8

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InactivateAsync_WhenAdjustmentDoesNotExist_ReturnsFalseWithoutTransaction()
    {
        // Arrange
        MockRepository
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StockLotAdjustment?)null);

        // Act
        var result = await StockLotAdjustmentService.InactivateAsync(999);

        // Assert
        Assert.False(result);

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        MockStockLotRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InactivateAsync_WhenAlreadyInactivated_ReturnsFalseWithoutTransaction()
    {
        // Arrange
        var adjustment = new StockLotAdjustment(10, 5, StockLotAdjustmentReasonEnum.Adjusted) { Id = 2 };
        adjustment.Inactivate();

        MockRepository
            .Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(adjustment);

        // Act
        var result = await StockLotAdjustmentService.InactivateAsync(2);

        // Assert
        Assert.False(result);

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
