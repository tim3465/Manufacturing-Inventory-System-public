using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Shifts;

public partial class ShiftTests
{
    [Fact]
    public async Task ListRunningShiftsAsync_WhenEmpty_ReturnsEmptyList()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int operatorId = 5;

        MockRepository.Setup(r => r.ListRunningByOperatorAsync(operatorId, ct)).ReturnsAsync(new List<Shift>());

        // Act
        var result = await ShiftService.ListRunningShiftsAsync(operatorId, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        MockRepository.Verify(r => r.ListRunningByOperatorAsync(operatorId, ct), Times.Once);
    }

    [Fact]
    public async Task ListRunningShiftsAsync_WhenShiftsExist_ReturnsMappedDtos()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int operatorId = 5;

        var shift = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 1, startTime: DateTime.UtcNow.AddHours(-1),
            partsMade: 5, scrap: 0);

        var job = new Job(orderId: 1, stockLotId: null, machineId: 7, partAmountPlanned: 100,
            barAmountPlanned: 10, barCycleTime: TimeSpan.FromMinutes(2), estimatedPartsPerBar: 5,
            dueDate: DateOnly.FromDateTime(DateTime.Today.AddDays(30)));

        typeof(Shift).GetProperty("Job")!.SetValue(shift, job);

        MockRepository.Setup(r => r.ListRunningByOperatorAsync(operatorId, ct)).ReturnsAsync(new List<Shift> { shift });

        // Act
        var result = await ShiftService.ListRunningShiftsAsync(operatorId, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(shift.Id, result[0].Id);
        Assert.Equal(shift.JobId, result[0].JobId);
        MockRepository.Verify(r => r.ListRunningByOperatorAsync(operatorId, ct), Times.Once);
    }

    [Fact]
    public async Task ListRunningShiftsAsync_WhenRunningShiftHasScrapAndIsNotInSiblingCollection_IncludesRunningShiftInJobTotals()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int operatorId = 5;

        // Closed sibling shift (simulating repository filter: StopTime != null only)
        var siblingShift = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 2, startTime: DateTime.UtcNow.AddHours(-5),
            partsMade: 8, scrap: 5, stopTime: DateTime.UtcNow.AddHours(-3));

        // Running shift with non-zero scrap — NOT added to job.Shifts
        var shift = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 1, startTime: DateTime.UtcNow.AddMinutes(-30),
            partsMade: 5, scrap: 15);

        var job = new Job(orderId: 1, stockLotId: null, machineId: 7, partAmountPlanned: 100,
            barAmountPlanned: 10, barCycleTime: TimeSpan.FromMinutes(2), estimatedPartsPerBar: 5,
            dueDate: DateOnly.FromDateTime(DateTime.Today.AddDays(30)));
        job.Shifts.Add(siblingShift);
        // current shift intentionally NOT added to job.Shifts — simulates repository filter (StopTime != null)

        typeof(Shift).GetProperty("Job")!.SetValue(shift, job);
        typeof(Shift).GetProperty("Job")!.SetValue(siblingShift, job);

        MockRepository.Setup(r => r.ListRunningByOperatorAsync(operatorId, ct)).ReturnsAsync(new List<Shift> { shift });

        // Act
        var result = await ShiftService.ListRunningShiftsAsync(operatorId, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(13, result[0].JobTotalPartsMade);   // 8 + 5 (running shift)
        Assert.Equal(20, result[0].JobTotalScrap);        // 5 + 15 (running shift)
        Assert.Equal(3, result[0].JobTotalBarsConsumed);  // 2 + 1 (running shift)
    }
}
