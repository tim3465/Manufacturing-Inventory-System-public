using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Shifts;

public partial class ShiftTests
{
    [Fact]
    public async Task ListShiftLogsAsync_WhenEmpty_ReturnsEmptyList()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int operatorId = 5;

        MockRepository.Setup(r => r.ListClosedByOperatorAsync(operatorId, ct)).ReturnsAsync(new List<Shift>());

        // Act
        var result = await ShiftService.ListShiftLogsAsync(operatorId, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        MockRepository.Verify(r => r.ListClosedByOperatorAsync(operatorId, ct), Times.Once);
    }

    [Fact]
    public async Task ListShiftLogsAsync_WhenShiftsExist_ReturnsMappedLogs()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int operatorId = 5;
        var startTime = DateTime.UtcNow.AddHours(-3);
        var stopTime = DateTime.UtcNow.AddHours(-1);

        var shift = new Shift(jobId: 2, operatorId: operatorId, barsConsumed: 2, startTime: startTime,
            partsMade: 10, scrap: 1, stopTime: stopTime);

        var job = new Job(orderId: 1, stockLotId: null, machineId: 7, partAmountPlanned: 100,
            barAmountPlanned: 10, barCycleTime: TimeSpan.FromMinutes(2), estimatedPartsPerBar: 5,
            dueDate: DateOnly.FromDateTime(DateTime.Today.AddDays(30)));

        typeof(Shift).GetProperty("Job")!.SetValue(shift, job);

        MockRepository.Setup(r => r.ListClosedByOperatorAsync(operatorId, ct)).ReturnsAsync(new List<Shift> { shift });

        // Act
        var result = await ShiftService.ListShiftLogsAsync(operatorId, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(shift.Id, result[0].Id);
        Assert.Equal(shift.PartsMade, result[0].PartsMade);
        Assert.Equal(shift.Scrap, result[0].Scrap);
        Assert.Equal(stopTime, result[0].StopTime);
        MockRepository.Verify(r => r.ListClosedByOperatorAsync(operatorId, ct), Times.Once);
    }
}
