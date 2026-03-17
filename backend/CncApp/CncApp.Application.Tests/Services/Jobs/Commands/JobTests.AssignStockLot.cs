using CncApp.Application.Dtos.Jobs;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Jobs;

public partial class JobTests
{
    [Fact]
    public async Task AssignStockLotAsync_WhenJobNotFound_ReturnsFalse()
    {
        // Arrange
        var jobId = 999;
        var dto = new AssignStockLotRequestDto { StockLotId = 1 };
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(jobId, cancellationToken))
            .ReturnsAsync((Job?)null);

        // Act
        var result = await JobService.AssignStockLotAsync(jobId, dto, cancellationToken);

        // Assert
        Assert.False(result);

        MockRepository.Verify(r => r.GetByIdAsync(jobId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
    }

    [Fact]
    public async Task AssignStockLotAsync_WhenJobFoundWithValidStockLotId_SetsPropertyAndReturnsTrue()
    {
        // Arrange
        var jobId = 1;
        var stockLotId = 42;
        var dto = new AssignStockLotRequestDto { StockLotId = stockLotId };
        var cancellationToken = CancellationToken.None;

        var job = new Job(
            orderId: 1,
            stockLotId: null,
            machineId: 1,
            partAmountPlanned: 10,
            barAmountPlanned: 5,
            barCycleTime: TimeSpan.FromSeconds(30),
            barsInJob: 5,
            estimatedPartsPerBar: 2,
            dueDate: DateOnly.FromDateTime(DateTime.Today.AddDays(7)));

        MockRepository
            .Setup(r => r.GetByIdAsync(jobId, cancellationToken))
            .ReturnsAsync(job);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await JobService.AssignStockLotAsync(jobId, dto, cancellationToken);

        // Assert
        Assert.True(result);
        Assert.Equal(stockLotId, job.StockLotId);

        MockRepository.Verify(r => r.GetByIdAsync(jobId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task AssignStockLotAsync_WhenJobFoundWithNullStockLotId_ClearsAssignmentAndReturnsTrue()
    {
        // Arrange
        var jobId = 1;
        var dto = new AssignStockLotRequestDto { StockLotId = null };
        var cancellationToken = CancellationToken.None;

        var job = new Job(
            orderId: 1,
            stockLotId: 5,
            machineId: 1,
            partAmountPlanned: 10,
            barAmountPlanned: 5,
            barCycleTime: TimeSpan.FromSeconds(30),
            barsInJob: 5,
            estimatedPartsPerBar: 2,
            dueDate: DateOnly.FromDateTime(DateTime.Today.AddDays(7)));

        MockRepository
            .Setup(r => r.GetByIdAsync(jobId, cancellationToken))
            .ReturnsAsync(job);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await JobService.AssignStockLotAsync(jobId, dto, cancellationToken);

        // Assert
        Assert.True(result);
        Assert.Null(job.StockLotId);

        MockRepository.Verify(r => r.GetByIdAsync(jobId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }
}
