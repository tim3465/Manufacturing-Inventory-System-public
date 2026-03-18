using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Machines;

public partial class MachineTests
{
    [Fact]
    public async Task ListActiveWithJobsAsync_WhenMachinesExist_ReturnsMappedDtos()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var part1 = new Part("Part One", "PN-001", TimeSpan.FromMinutes(1), 1) { Id = 1 };
        var part2 = new Part("Part Two", "PN-002", TimeSpan.FromMinutes(2), 1) { Id = 2 };

        var order1 = new Order(1, 1, 10) { Id = 1 };
        order1.Part = part1;
        var order2 = new Order(2, 1, 5) { Id = 2 };
        order2.Part = part2;

        var stockLot = new StockLot("LOT-001", 1, 10, 1.5m, 12m, CncApp.Domain.Enums.StockLotConditionEnum.AsReceived, DateTime.UtcNow) { Id = 1 };

        var jobEarlier = new Job(1, 1, 1, 10, 2, TimeSpan.FromMinutes(1), 5, new DateOnly(2026, 1, 15)) { Id = 10 };
        jobEarlier.Order = order1;
        jobEarlier.StockLot = stockLot;

        var jobLater = new Job(2, null, 1, 5, 1, TimeSpan.FromMinutes(2), 5, new DateOnly(2026, 2, 1)) { Id = 11 };
        jobLater.Order = order2;

        var machine = new Machine("SN-001", "MODEL-001") { Id = 1 };
        machine.Jobs = new List<Job> { jobLater, jobEarlier }; // intentionally out of order

        var machines = new List<Machine> { machine };

        MockRepository
            .Setup(r => r.ListActiveWithJobsAsync(cancellationToken))
            .ReturnsAsync(machines);

        // Act
        var result = await MachineService.ListActiveWithJobsAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);

        var dto = result[0];
        Assert.Equal(1, dto.Id);
        Assert.Equal("SN-001", dto.SerialNumber);
        Assert.Equal("MODEL-001", dto.ModelNumber);
        Assert.Equal(2, dto.Jobs.Count);

        // Jobs must be sorted by DueDate ascending
        Assert.Equal(10, dto.Jobs[0].Id);
        Assert.Equal("PN-001", dto.Jobs[0].PartNumber);
        Assert.Equal(new DateOnly(2026, 1, 15), dto.Jobs[0].DueDate);
        Assert.Equal("LOT-001", dto.Jobs[0].LotNumber);

        Assert.Equal(11, dto.Jobs[1].Id);
        Assert.Equal("PN-002", dto.Jobs[1].PartNumber);
        Assert.Equal(new DateOnly(2026, 2, 1), dto.Jobs[1].DueDate);
        Assert.Null(dto.Jobs[1].LotNumber);

        MockRepository.Verify(r => r.ListActiveWithJobsAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<It.IsAnyType>(It.IsAny<object>()), Times.Never);
    }

    [Fact]
    public async Task ListActiveWithJobsAsync_WhenNoMachines_ReturnsEmptyList()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.ListActiveWithJobsAsync(cancellationToken))
            .ReturnsAsync(new List<Machine>());

        // Act
        var result = await MachineService.ListActiveWithJobsAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        MockRepository.Verify(r => r.ListActiveWithJobsAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<It.IsAnyType>(It.IsAny<object>()), Times.Never);
    }

    [Fact]
    public async Task ListActiveWithJobsAsync_WhenMachineHasNoJobs_ReturnsEmptyJobsList()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var machine = new Machine("SN-002", "MODEL-002") { Id = 2 };
        machine.Jobs = new List<Job>();

        MockRepository
            .Setup(r => r.ListActiveWithJobsAsync(cancellationToken))
            .ReturnsAsync(new List<Machine> { machine });

        // Act
        var result = await MachineService.ListActiveWithJobsAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(2, result[0].Id);
        Assert.Empty(result[0].Jobs);

        MockRepository.Verify(r => r.ListActiveWithJobsAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<It.IsAnyType>(It.IsAny<object>()), Times.Never);
    }

    [Fact]
    public async Task ListActiveWithJobsAsync_WhenJobHasNullStockLot_ReturnsNullLotNumber()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var part = new Part("Part A", "PN-A", TimeSpan.FromMinutes(1), 1) { Id = 1 };
        var order = new Order(1, 1, 10) { Id = 1 };
        order.Part = part;

        var job = new Job(1, null, 1, 10, 2, TimeSpan.FromMinutes(1), 5, new DateOnly(2026, 3, 1)) { Id = 20 };
        job.Order = order;
        // StockLot is null (not set)

        var machine = new Machine("SN-003", "MODEL-003") { Id = 3 };
        machine.Jobs = new List<Job> { job };

        MockRepository
            .Setup(r => r.ListActiveWithJobsAsync(cancellationToken))
            .ReturnsAsync(new List<Machine> { machine });

        // Act
        var result = await MachineService.ListActiveWithJobsAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Single(result[0].Jobs);
        Assert.Null(result[0].Jobs[0].LotNumber);

        MockRepository.Verify(r => r.ListActiveWithJobsAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<It.IsAnyType>(It.IsAny<object>()), Times.Never);
    }
}
