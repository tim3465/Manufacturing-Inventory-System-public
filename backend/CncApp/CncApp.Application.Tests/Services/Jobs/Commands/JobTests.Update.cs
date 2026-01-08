using CncApp.Application.Dtos.Jobs;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Jobs;

public partial class JobTests
{
    [Fact]
    public async Task UpdateAsync_WhenJobExists_UpdatesAndReturnsJobDto()
    {
        // Arrange
        var jobId = 1;
        var dto = new UpdateJobRequestDto
        {
            MachineId = 10,
            StockLotId = 20,
            PartAmountPlanned = 30,
            BarAmountPlanned = 40,
            BarCycleTime = TimeSpan.FromMinutes(2),
            BarsInJob = 3,
            EstimatedPartsPerBar = 7
        };
        var cancellationToken = CancellationToken.None;

        var job = new Job(
            orderId: 1,
            stockLotId: 2,
            machineId: 3,
            partAmountPlanned: 4,
            barAmountPlanned: 5,
            barCycleTime: TimeSpan.FromMinutes(1),
            barsInJob: 1,
            estimatedPartsPerBar: null)
        {
            Id = jobId
        };

        var expectedDto = new JobDto
        {
            Id = jobId,
            OrderId = job.OrderId,
            StockLotId = dto.StockLotId.Value,
            MachineId = dto.MachineId.Value,
            PartAmountPlanned = dto.PartAmountPlanned.Value,
            BarAmountPlanned = dto.BarAmountPlanned.Value,
            BarCycleTime = dto.BarCycleTime.Value,
            BarsInJob = dto.BarsInJob.Value,
            EstimatedPartsPerBar = dto.EstimatedPartsPerBar.Value
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(jobId, cancellationToken))
            .ReturnsAsync(job);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        MockMapper
            .Setup(m => m.Map<JobDto>(It.IsAny<Job>()))
            .Returns(expectedDto);

        // Act
        var result = await JobService.UpdateAsync(jobId, dto, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(jobId, result.Id);
        Assert.Equal(dto.MachineId.Value, job.MachineId);
        Assert.Equal(dto.StockLotId.Value, job.StockLotId);
        Assert.Equal(dto.PartAmountPlanned.Value, job.PartAmountPlanned);
        Assert.Equal(dto.BarAmountPlanned.Value, job.BarAmountPlanned);
        Assert.Equal(dto.BarCycleTime.Value, job.BarCycleTime);
        Assert.Equal(dto.BarsInJob.Value, job.BarsInJob);
        Assert.Equal(dto.EstimatedPartsPerBar.Value, job.EstimatedPartsPerBar);

        MockRepository.Verify(r => r.GetByIdAsync(jobId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<JobDto>(It.IsAny<Job>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenJobDoesNotExist_ReturnsNull()
    {
        // Arrange
        var jobId = 999;
        var dto = new UpdateJobRequestDto
        {
            MachineId = 10
        };
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(jobId, cancellationToken))
            .ReturnsAsync((Job?)null);

        // Act
        var result = await JobService.UpdateAsync(jobId, dto, cancellationToken);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(jobId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
        MockMapper.Verify(m => m.Map<JobDto>(It.IsAny<Job>()), Times.Never);
    }
}

