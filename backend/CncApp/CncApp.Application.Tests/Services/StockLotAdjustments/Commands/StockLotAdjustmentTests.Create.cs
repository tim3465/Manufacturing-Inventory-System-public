using CncApp.Application.Dtos.StockLotAdjustments;
using CncApp.Domain.Entities;
using CncApp.Domain.Enums;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.StockLotAdjustments;

public partial class StockLotAdjustmentTests
{
    // ── public CreateAsync (controller path) ─────────────────────────────

    [Fact]
    public async Task CreateAsync_BeginsAndCommitsTransaction()
    {
        // Arrange
        var dto = new CreateStockLotAdjustmentRequestDto
        {
            StockLotId = 1,
            DeltaBars = 10,
            Reason = StockLotAdjustmentReasonEnum.Received,
            Notes = "Test notes"
        };

        var adjustment = new StockLotAdjustment(dto.StockLotId, dto.DeltaBars, dto.Reason, notes: dto.Notes)
        {
            Id = 42
        };

        var stockLot = new StockLot("LOT-001", 1, 5, 25m, 3000m,
            StockLotConditionEnum.AsReceived, DateTime.UtcNow) { Id = 1 };

        MockMapper
            .Setup(m => m.Map<StockLotAdjustment>(dto))
            .Returns(adjustment);
        MockRepository
            .Setup(r => r.AddAsync(It.IsAny<StockLotAdjustment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockStockLotRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
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
        var result = await StockLotAdjustmentService.CreateAsync(dto);

        // Assert
        Assert.Equal(42, result);
        Assert.Equal(15, stockLot.AmountOfBars); // 5 + 10

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenFailure_RollsBackTransaction()
    {
        // Arrange
        var dto = new CreateStockLotAdjustmentRequestDto
        {
            StockLotId = 3,
            DeltaBars = 7,
            Reason = StockLotAdjustmentReasonEnum.Scrap
        };

        MockMapper
            .Setup(m => m.Map<StockLotAdjustment>(It.IsAny<CreateStockLotAdjustmentRequestDto>()))
            .Throws(new InvalidOperationException("Simulated failure"));
        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => StockLotAdjustmentService.CreateAsync(dto));

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
