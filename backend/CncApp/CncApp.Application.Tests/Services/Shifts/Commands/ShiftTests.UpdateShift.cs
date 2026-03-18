using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Shifts;

public partial class ShiftTests
{
    [Fact]
    public async Task UpdateShiftAsync_WhenShiftNotFound_ReturnsFalse()
    {
        // Arrange
        var dto = new UpdateShiftRequestDto { StartTime = DateTime.UtcNow, PartsMade = 10, Scrap = 0, BarsConsumed = 1 };
        var ct = CancellationToken.None;

        MockRepository.Setup(r => r.GetByIdAsync(99, ct)).ReturnsAsync((Shift?)null);

        // Act
        var result = await ShiftService.UpdateShiftAsync(99, operatorId: 1, dto, ct);

        // Assert
        Assert.False(result);
        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task UpdateShiftAsync_WhenWrongOperator_ThrowsInvalidOperationException()
    {
        // Arrange
        var dto = new UpdateShiftRequestDto { StartTime = DateTime.UtcNow, PartsMade = 10, Scrap = 0, BarsConsumed = 1 };
        var ct = CancellationToken.None;

        var shift = new Shift(jobId: 1, operatorId: 5, barsConsumed: 0, startTime: DateTime.UtcNow.AddHours(-1));
        MockRepository.Setup(r => r.GetByIdAsync(shift.Id, ct)).ReturnsAsync(shift);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ShiftService.UpdateShiftAsync(shift.Id, operatorId: 99, dto, ct));

        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task UpdateShiftAsync_WhenShiftAlreadyClosed_ThrowsInvalidOperationException()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(-2);
        var stopTime = DateTime.UtcNow.AddHours(-1);
        var dto = new UpdateShiftRequestDto { StartTime = startTime, PartsMade = 10, Scrap = 0, BarsConsumed = 1 };
        var ct = CancellationToken.None;
        const int operatorId = 5;

        var shift = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 0, startTime: startTime, stopTime: stopTime);
        MockRepository.Setup(r => r.GetByIdAsync(shift.Id, ct)).ReturnsAsync(shift);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ShiftService.UpdateShiftAsync(shift.Id, operatorId, dto, ct));

        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task UpdateShiftAsync_WhenValid_UpdatesAndReturnsTrue()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(-1);
        var dto = new UpdateShiftRequestDto
        {
            StartTime = startTime,
            PartsMade = 15,
            Scrap = 2,
            BarsConsumed = 3,
            PartsPerBar = 5,
            Downtime = TimeSpan.FromMinutes(10)
        };
        var ct = CancellationToken.None;
        const int operatorId = 5;

        var shift = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 0, startTime: startTime);
        MockRepository.Setup(r => r.GetByIdAsync(shift.Id, ct)).ReturnsAsync(shift);
        MockRepository.Setup(r => r.SaveChangesAsync(ct)).Returns(Task.CompletedTask);

        // Act
        var result = await ShiftService.UpdateShiftAsync(shift.Id, operatorId, dto, ct);

        // Assert
        Assert.True(result);
        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Once);
    }
}
