using CncApp.Application.Dtos.Jobs;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Jobs;

public partial class JobTests
{
    [Fact]
    public async Task GetAsync_WhenJobExists_ReturnsJobDto()
    {
        // Arrange
        var jobId = 1;
        var cancellationToken = CancellationToken.None;

        var job = new Job(
            orderId: 1,
            stockLotId: 2,
            machineId: 3,
            partAmountPlanned: 10,
            barAmountPlanned: 5,
            barCycleTime: TimeSpan.FromMinutes(1),
            estimatedPartsPerBar: 5,
            dueDate: new DateOnly(2026, 6, 1))
        {
            Id = jobId
        };

        var expectedDto = new JobDto
        {
            Id = jobId,
            OrderId = job.OrderId,
            StockLotId = job.StockLotId,
            MachineId = job.MachineId,
            PartAmountPlanned = job.PartAmountPlanned,
            BarAmountPlanned = job.BarAmountPlanned,
            BarCycleTime = job.BarCycleTime,
            BarsInJob = job.BarsInJob,
            EstimatedPartsPerBar = job.EstimatedPartsPerBar
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(jobId, cancellationToken))
            .ReturnsAsync(job);

        MockMapper
            .Setup(m => m.Map<JobDto>(job))
            .Returns(expectedDto);

        // Act
        var result = await JobService.GetAsync(jobId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(jobId, result.Id);

        MockRepository.Verify(r => r.GetByIdAsync(jobId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<JobDto>(job), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenJobDoesNotExist_ReturnsNull()
    {
        // Arrange
        var jobId = 999;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(jobId, cancellationToken))
            .ReturnsAsync((Job?)null);

        // Act
        var result = await JobService.GetAsync(jobId, cancellationToken);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(jobId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<JobDto>(It.IsAny<Job>()), Times.Never);
    }
}

