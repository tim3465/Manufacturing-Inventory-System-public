using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Shifts;

public partial class ShiftTests
{
    [Fact]
    public async Task GetRunningShiftAsync_WhenShiftNotFound_ReturnsNull()
    {
        // Arrange
        var ct = CancellationToken.None;
        MockRepository.Setup(r => r.GetRunningShiftWithContextAsync(99, ct)).ReturnsAsync((Shift?)null);

        // Act
        var result = await ShiftService.GetRunningShiftAsync(99, operatorId: 1, ct);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetRunningShiftAsync_WhenWrongOperator_ReturnsNull()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int operatorId = 5;
        const int differentOperatorId = 99;

        var shift = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 0, startTime: DateTime.UtcNow.AddHours(-1));

        MockRepository.Setup(r => r.GetRunningShiftWithContextAsync(shift.Id, ct)).ReturnsAsync(shift);

        // Act
        var result = await ShiftService.GetRunningShiftAsync(shift.Id, differentOperatorId, ct);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetRunningShiftAsync_WhenValid_ReturnsDtoWithCorrectTotals()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int operatorId = 5;
        const int shiftId = 10;

        // Two closed sibling shifts
        var siblingShift1 = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 2, startTime: DateTime.UtcNow.AddHours(-5),
            partsMade: 8, scrap: 1, stopTime: DateTime.UtcNow.AddHours(-3));
        var siblingShift2 = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 3, startTime: DateTime.UtcNow.AddHours(-3),
            partsMade: 12, scrap: 2, stopTime: DateTime.UtcNow.AddHours(-1));

        // Running shift (no partsMade/scrap/barsConsumed yet, so contributes 0 to totals)
        var shift = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 0, startTime: DateTime.UtcNow.AddMinutes(-30));
        shift.GetType().GetProperty("Id")!.SetValue(shift, shiftId);

        // Build a minimal job with navigation objects via reflection (avoids domain constructor complications)
        var job = new Job(orderId: 1, stockLotId: null, machineId: 7, partAmountPlanned: 100,
            barAmountPlanned: 10, barCycleTime: TimeSpan.FromMinutes(2), estimatedPartsPerBar: 5,
            dueDate: DateOnly.FromDateTime(DateTime.Today.AddDays(30)));
        job.Shifts.Add(siblingShift1);
        job.Shifts.Add(siblingShift2);
        job.Shifts.Add(shift);

        // Wire job onto shift via reflection (navigation property)
        typeof(Shift).GetProperty("Job")!.SetValue(shift, job);
        typeof(Shift).GetProperty("Job")!.SetValue(siblingShift1, job);
        typeof(Shift).GetProperty("Job")!.SetValue(siblingShift2, job);

        MockRepository.Setup(r => r.GetRunningShiftWithContextAsync(shiftId, ct)).ReturnsAsync(shift);

        // Act
        var result = await ShiftService.GetRunningShiftAsync(shiftId, operatorId, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(shiftId, result.Id);
        Assert.Equal(20, result.JobTotalPartsMade);   // 8 + 12 + 0 (running shift already in collection)
        Assert.Equal(3, result.JobTotalScrap);         // 1 + 2 + 0 (running shift already in collection)
        Assert.Equal(5, result.JobTotalBarsConsumed);  // 2 + 3 + 0 (running shift already in collection)
    }

    [Fact]
    public async Task GetRunningShiftAsync_WhenRunningShiftHasScrapAndIsNotInSiblingCollection_IncludesRunningShiftInJobTotals()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int operatorId = 5;
        const int shiftId = 10;

        // Two closed sibling shifts (simulating what the repository actually loads: StopTime != null only)
        var siblingShift1 = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 2, startTime: DateTime.UtcNow.AddHours(-5),
            partsMade: 8, scrap: 1, stopTime: DateTime.UtcNow.AddHours(-3));
        var siblingShift2 = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 3, startTime: DateTime.UtcNow.AddHours(-3),
            partsMade: 12, scrap: 2, stopTime: DateTime.UtcNow.AddHours(-1));

        // Running shift with non-zero scrap — NOT added to job.Shifts (mirrors real repository behavior)
        var shift = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 1, startTime: DateTime.UtcNow.AddMinutes(-30),
            partsMade: 5, scrap: 15);
        shift.GetType().GetProperty("Id")!.SetValue(shift, shiftId);

        var job = new Job(orderId: 1, stockLotId: null, machineId: 7, partAmountPlanned: 100,
            barAmountPlanned: 10, barCycleTime: TimeSpan.FromMinutes(2), estimatedPartsPerBar: 5,
            dueDate: DateOnly.FromDateTime(DateTime.Today.AddDays(30)));
        job.Shifts.Add(siblingShift1);
        job.Shifts.Add(siblingShift2);
        // current shift intentionally NOT added to job.Shifts — simulates repository filter (StopTime != null)

        typeof(Shift).GetProperty("Job")!.SetValue(shift, job);
        typeof(Shift).GetProperty("Job")!.SetValue(siblingShift1, job);
        typeof(Shift).GetProperty("Job")!.SetValue(siblingShift2, job);

        MockRepository.Setup(r => r.GetRunningShiftWithContextAsync(shiftId, ct)).ReturnsAsync(shift);

        // Act
        var result = await ShiftService.GetRunningShiftAsync(shiftId, operatorId, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(25, result.JobTotalPartsMade);   // 8 + 12 + 5 (running shift)
        Assert.Equal(18, result.JobTotalScrap);        // 1 + 2 + 15 (running shift)
        Assert.Equal(6, result.JobTotalBarsConsumed);  // 2 + 3 + 1 (running shift)
    }
}
