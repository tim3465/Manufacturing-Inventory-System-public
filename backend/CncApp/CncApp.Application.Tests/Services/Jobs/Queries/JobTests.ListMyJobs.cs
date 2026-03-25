using CncApp.Application.Dtos.Jobs;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Jobs;

public partial class JobTests
{
    [Fact]
    public async Task ListMyJobsAsync_WhenJobsExist_ReturnsListOfMyJobListItemDtos()
    {
        // Arrange
        var operatorId = 1;
        var ct = CancellationToken.None;

        var machine1 = new Machine("SN-001", "Model-A") { Id = 10 };
        var machine2 = new Machine("SN-002", "Model-B") { Id = 11 };

        var part1 = new Part("Widget", "P-001", TimeSpan.FromMinutes(1), 5) { Id = 20 };
        var part2 = new Part("Gadget", "P-002", TimeSpan.FromMinutes(2), 3) { Id = 21 };

        var order1 = new Order(part1.Id, 1, 100) { Id = 30, Part = part1 };
        var order2 = new Order(part2.Id, 1, 50) { Id = 31, Part = part2 };

        var job1 = new Job(order1.Id, null, machine1.Id, 100, 10, TimeSpan.FromMinutes(1), 5, new DateOnly(2026, 6, 1))
        {
            Id = 1,
            Machine = machine1,
            Order = order1
        };

        var job2 = new Job(order2.Id, null, machine2.Id, 50, 5, TimeSpan.FromMinutes(2), 3, new DateOnly(2026, 7, 1))
        {
            Id = 2,
            Machine = machine2,
            Order = order2
        };

        var jobs = new List<Job> { job1, job2 };

        MockRepository
            .Setup(r => r.ListByOperatorAsync(operatorId, ct))
            .ReturnsAsync(jobs);

        // Act
        var result = await JobService.ListMyJobsAsync(operatorId, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Equal(1, result[0].Id);
        Assert.Equal("1", result[0].JobNumber);
        Assert.Equal("P-001", result[0].PartNumber);
        Assert.Equal("Widget", result[0].PartName);
        Assert.Equal("SN-001", result[0].MachineName);

        Assert.Equal(2, result[1].Id);
        Assert.Equal("2", result[1].JobNumber);
        Assert.Equal("P-002", result[1].PartNumber);
        Assert.Equal("Gadget", result[1].PartName);
        Assert.Equal("SN-002", result[1].MachineName);

        MockRepository.Verify(r => r.ListByOperatorAsync(operatorId, ct), Times.Once);
        MockMapper.Verify(m => m.Map<List<JobShiftDto>>(It.IsAny<object>()), Times.Never);
    }

    [Fact]
    public async Task ListMyJobsAsync_WhenNoJobsExist_ReturnsEmptyList()
    {
        // Arrange
        var operatorId = 1;
        var ct = CancellationToken.None;

        MockRepository
            .Setup(r => r.ListByOperatorAsync(operatorId, ct))
            .ReturnsAsync(new List<Job>());

        // Act
        var result = await JobService.ListMyJobsAsync(operatorId, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        MockRepository.Verify(r => r.ListByOperatorAsync(operatorId, ct), Times.Once);
    }
}
