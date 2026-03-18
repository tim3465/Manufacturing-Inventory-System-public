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
}
