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

        var closedShift1 = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 2, startTime: DateTime.UtcNow.AddHours(-5),
            partsMade: 8, scrap: 1, stopTime: DateTime.UtcNow.AddHours(-3));
        var closedShift2 = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 3, startTime: DateTime.UtcNow.AddHours(-3),
            partsMade: 12, scrap: 2, stopTime: DateTime.UtcNow.AddHours(-1));

        var shift = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 0, startTime: DateTime.UtcNow.AddMinutes(-30));
        shift.GetType().GetProperty("Id")!.SetValue(shift, shiftId);

        // Build a minimal job with navigation objects via reflection (avoids domain constructor complications)
        var job = new Job(orderId: 1, stockLotId: null, machineId: 7, partAmountPlanned: 100,
            barAmountPlanned: 10, barCycleTime: TimeSpan.FromMinutes(2), estimatedPartsPerBar: 5,
            dueDate: DateOnly.FromDateTime(DateTime.Today.AddDays(30)));
        job.Shifts.Add(closedShift1);
        job.Shifts.Add(closedShift2);
        job.Shifts.Add(shift);

        // Wire job onto shift via reflection (navigation property)
        typeof(Shift).GetProperty("Job")!.SetValue(shift, job);
        typeof(Shift).GetProperty("Job")!.SetValue(closedShift1, job);
        typeof(Shift).GetProperty("Job")!.SetValue(closedShift2, job);

        MockRepository.Setup(r => r.GetRunningShiftWithContextAsync(shiftId, ct)).ReturnsAsync(shift);

        // Act
        var result = await ShiftService.GetRunningShiftAsync(shiftId, operatorId, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(shiftId, result.Id);
        Assert.Equal(20, result.JobTotalPartsMade);   // 8 + 12
        Assert.Equal(3, result.JobTotalScrap);         // 1 + 2
        Assert.Equal(5, result.JobTotalBarsConsumed);  // 2 + 3
    }
}
