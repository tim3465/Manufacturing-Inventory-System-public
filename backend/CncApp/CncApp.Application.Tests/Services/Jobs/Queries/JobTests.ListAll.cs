using CncApp.Application.Dtos.Jobs;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Jobs;

public partial class JobTests
{
    [Fact]
    public async Task ListAllAsync_WhenJobsExist_ReturnsListOfJobDtos()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var jobs = new List<Job>
        {
            new Job(1, 1, 1, 10, 5, TimeSpan.FromMinutes(1), 2, 5, new DateOnly(2026, 6, 1)) { Id = 1 },
            new Job(2, 2, 2, 20, 10, TimeSpan.FromMinutes(2), 3, 6, new DateOnly(2026, 7, 1)) { Id = 2 }
        };

        var expectedDtos = new List<JobDto>
        {
            new JobDto { Id = 1, OrderId = 1, StockLotId = 1, MachineId = 1, PartAmountPlanned = 10, BarAmountPlanned = 5, BarCycleTime = TimeSpan.FromMinutes(1), BarsInJob = 2, EstimatedPartsPerBar = 5 },
            new JobDto { Id = 2, OrderId = 2, StockLotId = 2, MachineId = 2, PartAmountPlanned = 20, BarAmountPlanned = 10, BarCycleTime = TimeSpan.FromMinutes(2), BarsInJob = 3, EstimatedPartsPerBar = 6 }
        };

        MockRepository
            .Setup(r => r.ListAllAsync(cancellationToken))
            .ReturnsAsync(jobs);

        MockMapper
            .Setup(m => m.Map<List<JobDto>>(jobs))
            .Returns(expectedDtos);

        // Act
        var result = await JobService.ListAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);

        MockRepository.Verify(r => r.ListAllAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<JobDto>>(jobs), Times.Once);
    }

    [Fact]
    public async Task ListAllAsync_WhenNoJobsExist_ReturnsEmptyList()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var jobs = new List<Job>();

        MockRepository
            .Setup(r => r.ListAllAsync(cancellationToken))
            .ReturnsAsync(jobs);

        MockMapper
            .Setup(m => m.Map<List<JobDto>>(jobs))
            .Returns(new List<JobDto>());

        // Act
        var result = await JobService.ListAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        MockRepository.Verify(r => r.ListAllAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<JobDto>>(jobs), Times.Once);
    }
}

