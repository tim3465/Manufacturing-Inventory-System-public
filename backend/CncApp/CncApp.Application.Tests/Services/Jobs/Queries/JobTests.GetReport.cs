using CncApp.Domain.Entities;
using CncApp.Domain.Enums;
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

        // Issue logs across both shifts
        var log1 = new ShiftIssueLog(shiftId: 100, issueType: IssueTypeEnum.Setup, scrapQuantity: 1, description: "Tool broke")
        {
            Id = 200,
            CreatedDateTime = new DateTimeOffset(2026, 3, 1, 10, 0, 0, TimeSpan.Zero)
        };
        var log2 = new ShiftIssueLog(shiftId: 101, issueType: IssueTypeEnum.Production, scrapQuantity: 0, description: "Coolant leak", downtime: TimeSpan.FromMinutes(10))
        {
            Id = 201,
            CreatedDateTime = new DateTimeOffset(2026, 3, 2, 9, 0, 0, TimeSpan.Zero)
        };
        var log3 = new ShiftIssueLog(shiftId: 100, issueType: IssueTypeEnum.Production, scrapQuantity: 2, description: "Material defect")
        {
            Id = 202,
            CreatedDateTime = new DateTimeOffset(2026, 3, 1, 14, 0, 0, TimeSpan.Zero)
        };

        shift1.ShiftIssueLogs = new List<ShiftIssueLog> { log1, log3 };
        shift2.ShiftIssueLogs = new List<ShiftIssueLog> { log2 };

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

        // Issue logs merged and sorted chronologically
        Assert.Equal(3, result.IssueLogs.Count);
        Assert.Equal(200, result.IssueLogs[0].Id); // log1: 2026-03-01 10:00
        Assert.Equal(202, result.IssueLogs[1].Id); // log3: 2026-03-01 14:00
        Assert.Equal(201, result.IssueLogs[2].Id); // log2: 2026-03-02 09:00

        // Verify operator names come from the shift's operator
        Assert.Equal("John Doe", result.IssueLogs[0].OperatorName);   // shift1 operator
        Assert.Equal("John Doe", result.IssueLogs[1].OperatorName);   // shift1 operator
        Assert.Equal("Jane Smith", result.IssueLogs[2].OperatorName); // shift2 operator

        // Verify shift IDs
        Assert.Equal(100, result.IssueLogs[0].ShiftId);
        Assert.Equal(100, result.IssueLogs[1].ShiftId);
        Assert.Equal(101, result.IssueLogs[2].ShiftId);

        // Verify issue log fields
        Assert.Equal(IssueTypeEnum.Setup, result.IssueLogs[0].IssueType);
        Assert.Equal("Tool broke", result.IssueLogs[0].Description);
        Assert.Equal(1, result.IssueLogs[0].ScrapQuantity);

        MockRepository.Verify(r => r.GetByIdWithShiftsAsync(jobId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetReportAsync_WhenJobHasNoIssueLogs_ReturnsEmptyIssueLogsList()
    {
        // Arrange
        var jobId = 2;
        var cancellationToken = CancellationToken.None;

        var part = new Part("Bolt", "BLT-001", TimeSpan.FromMinutes(1), 10) { Id = 2 };
        var order = new Order(partId: 2, customerId: 1, partAmountRequested: 50) { Id = 2 };
        order.Part = part;

        var job = new Job(
            orderId: 2,
            stockLotId: 1,
            machineId: 1,
            partAmountPlanned: 50,
            barAmountPlanned: 5,
            barCycleTime: TimeSpan.FromMinutes(3),
            estimatedPartsPerBar: 10,
            dueDate: new DateOnly(2026, 7, 1))
        {
            Id = jobId,
            Machine = new Machine("CNC-002", "MODEL-B") { Id = 1 },
            Order = order
        };

        var shift = new Shift(
            jobId: jobId,
            operatorId: 10,
            barsConsumed: 2,
            startTime: new DateTime(2026, 4, 1, 8, 0, 0),
            partsMade: 20,
            scrap: 0,
            partsPerBar: 10,
            stopTime: new DateTime(2026, 4, 1, 16, 0, 0),
            downtime: null)
        {
            Id = 300,
            Operator = new User { Id = 10, UserName = "op1", FirstName = "John", LastName = "Doe" },
            ShiftIssueLogs = new List<ShiftIssueLog>()
        };

        job.Shifts = new List<Shift> { shift };

        MockRepository
            .Setup(r => r.GetByIdWithShiftsAsync(jobId, cancellationToken))
            .ReturnsAsync(job);

        // Act
        var result = await JobService.GetReportAsync(jobId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.IssueLogs);
        Assert.Single(result.Shifts);

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
