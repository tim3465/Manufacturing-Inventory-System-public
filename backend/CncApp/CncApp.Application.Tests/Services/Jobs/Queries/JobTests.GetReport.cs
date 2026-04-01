using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Jobs;

public partial class JobTests
{
    [Fact]
    public async Task GetReportAsync_WhenJobExists_ReturnsReportWithCorrectTotals()
    {
        // Arrange
        var jobId = 1;
        var cancellationToken = CancellationToken.None;

        var operator1 = new User { Id = 10, UserName = "op1", FirstName = "John", LastName = "Doe" };
        var operator2 = new User { Id = 11, UserName = "op2", FirstName = "Jane", LastName = "Smith" };

        var part = new Part("Widget", "WDG-001", TimeSpan.FromMinutes(2), 5) { Id = 1 };
        var order = new Order(partId: 1, customerId: 1, partAmountRequested: 100) { Id = 1 };
        order.Part = part;

        var job = new Job(
            orderId: 1,
            stockLotId: 2,
            machineId: 3,
            partAmountPlanned: 100,
            barAmountPlanned: 20,
            barCycleTime: TimeSpan.FromMinutes(5),
            estimatedPartsPerBar: 5,
            dueDate: new DateOnly(2026, 6, 1))
        {
            Id = jobId,
            Machine = new Machine("CNC-001", "MODEL-A") { Id = 3 },
            Order = order
        };

        var shift1 = new Shift(
            jobId: jobId,
            operatorId: 10,
            barsConsumed: 5,
            startTime: new DateTime(2026, 3, 1, 8, 0, 0),
            partsMade: 25,
            scrap: 2,
            partsPerBar: 5,
            stopTime: new DateTime(2026, 3, 1, 16, 0, 0),
            downtime: TimeSpan.FromMinutes(30))
        {
            Id = 100,
            Operator = operator1
        };

        var shift2 = new Shift(
            jobId: jobId,
            operatorId: 11,
            barsConsumed: 3,
            startTime: new DateTime(2026, 3, 2, 8, 0, 0),
            partsMade: 15,
            scrap: 1,
            partsPerBar: 5,
            stopTime: new DateTime(2026, 3, 2, 16, 0, 0),
            downtime: TimeSpan.FromMinutes(15))
        {
            Id = 101,
            Operator = operator2
        };

        job.Shifts = new List<Shift> { shift1, shift2 };

        MockRepository
            .Setup(r => r.GetByIdWithShiftsAsync(jobId, cancellationToken))
            .ReturnsAsync(job);

        // Act
        var result = await JobService.GetReportAsync(jobId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(jobId, result.Id);
        Assert.Equal(1, result.OrderId);
        Assert.Equal("CNC-001", result.MachineName);
        Assert.Equal("Widget", result.PartName);
        Assert.Equal("WDG-001", result.PartNumber);
        Assert.Equal(new DateOnly(2026, 6, 1), result.DueDate);
        Assert.Equal("Planned", result.JobStatus);

        // Aggregated totals
        Assert.Equal(100, result.PartAmountPlanned);
        Assert.Equal(40, result.TotalPartsMade);       // 25 + 15
        Assert.Equal(3, result.TotalScrap);             // 2 + 1
        Assert.Equal(20, result.BarAmountPlanned);
        Assert.Equal(8, result.TotalBarsConsumed);      // 5 + 3
        Assert.Equal(5, result.EstimatedPartsPerBar);
        Assert.Equal(5.00m, result.ActualPartsPerBar);  // 40 / 8 = 5.00
        Assert.Equal(TimeSpan.FromMinutes(45), result.TotalDowntime); // 30 + 15

        // Shifts ordered by StartTime descending
        Assert.Equal(2, result.Shifts.Count);
        Assert.Equal(101, result.Shifts[0].Id); // shift2 (2026-03-02) first
        Assert.Equal(100, result.Shifts[1].Id); // shift1 (2026-03-01) second
        Assert.Equal("Jane Smith", result.Shifts[0].OperatorName);
        Assert.Equal("John Doe", result.Shifts[1].OperatorName);

        MockRepository.Verify(r => r.GetByIdWithShiftsAsync(jobId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetReportAsync_WhenJobNotFound_ReturnsNull()
    {
        // Arrange
        var jobId = 999;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdWithShiftsAsync(jobId, cancellationToken))
            .ReturnsAsync((Job?)null);

        // Act
        var result = await JobService.GetReportAsync(jobId, cancellationToken);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdWithShiftsAsync(jobId, cancellationToken), Times.Once);
    }
}
