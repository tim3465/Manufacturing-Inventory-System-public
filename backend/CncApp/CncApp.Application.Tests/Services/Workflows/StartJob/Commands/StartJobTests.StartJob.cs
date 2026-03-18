using CncApp.Application.Dtos.Jobs;
using CncApp.Application.Dtos.StockLotAdjustments;
using CncApp.Domain.Entities;
using CncApp.Domain.Enums;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Workflows.StartJob;

public partial class StartJobTests
{
    [Fact]
    public async Task StartJobAsync_WhenValidJobAndBars_CommitsAndReturnsResponse()
    {
        // Arrange
        var job = new Job(
            orderId: 1,
            stockLotId: 10,
            machineId: 1,
            partAmountPlanned: 100,
            barAmountPlanned: 20,
            barCycleTime: TimeSpan.FromMinutes(1),
            estimatedPartsPerBar: 5,
            dueDate: new DateOnly(2026, 6, 1)) { Id = 42 };

        var stockLot = new StockLot("LOT-001", 1, 50, 25.4m, 3000m,
            StockLotConditionEnum.AsReceived, new DateTime(2026, 1, 1)) { Id = 10 };

        var adjustment = new StockLotAdjustment(10, -5, StockLotAdjustmentReasonEnum.JobStart, jobId: 42) { Id = 99 };

        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockJobRepository
            .Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);
        MockJobRepository
            .Setup(r => r.GetActiveJobByMachineAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Job?)null);
        MockJobRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockMapper
            .Setup(m => m.Map<StockLotAdjustment>(It.IsAny<CreateStockLotAdjustmentRequestDto>()))
            .Returns(adjustment);

        MockStockLotAdjustmentRepository
            .Setup(r => r.AddAsync(It.IsAny<StockLotAdjustment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockStockLotAdjustmentRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockStockLotRepository
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stockLot);
        MockStockLotRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var dto = new StartJobRequestDto { BarsToAdd = 5 };

        // Act
        var result = await Service.StartJobAsync(42, dto);

        // Assert
        Assert.Equal(42, result.JobId);
        Assert.Equal(99, result.StockLotAdjustmentId);

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StartJobAsync_WhenJobNotFound_RollsBackAndThrows()
    {
        // Arrange
        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockJobRepository
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Job?)null);

        var dto = new StartJobRequestDto { BarsToAdd = 5 };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => Service.StartJobAsync(999, dto));

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StartJobAsync_WhenJobHasNoStockLot_RollsBackAndThrows()
    {
        // Arrange
        var job = new Job(
            orderId: 1,
            stockLotId: null,
            machineId: 1,
            partAmountPlanned: 100,
            barAmountPlanned: 20,
            barCycleTime: TimeSpan.FromMinutes(1),
            estimatedPartsPerBar: 5,
            dueDate: new DateOnly(2026, 6, 1)) { Id = 5 };

        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockJobRepository
            .Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);

        var dto = new StartJobRequestDto { BarsToAdd = 5 };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => Service.StartJobAsync(5, dto));

        Assert.Contains("no stock lot", ex.Message);

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StartJobAsync_WhenMachineAlreadyHasActiveJob_RollsBackAndThrows()
    {
        // Arrange
        var job = new Job(
            orderId: 1,
            stockLotId: 10,
            machineId: 3,
            partAmountPlanned: 100,
            barAmountPlanned: 20,
            barCycleTime: TimeSpan.FromMinutes(1),
            estimatedPartsPerBar: 5,
            dueDate: new DateOnly(2026, 6, 1)) { Id = 7 };

        var existingActiveJob = new Job(
            orderId: 2,
            stockLotId: 11,
            machineId: 3,
            partAmountPlanned: 50,
            barAmountPlanned: 10,
            barCycleTime: TimeSpan.FromMinutes(2),
            estimatedPartsPerBar: 5,
            dueDate: new DateOnly(2026, 5, 1)) { Id = 8 };

        existingActiveJob.Start(5);

        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockJobRepository
            .Setup(r => r.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);
        MockJobRepository
            .Setup(r => r.GetActiveJobByMachineAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingActiveJob);

        var dto = new StartJobRequestDto { BarsToAdd = 5 };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => Service.StartJobAsync(7, dto));

        Assert.Contains("active job", ex.Message);

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
