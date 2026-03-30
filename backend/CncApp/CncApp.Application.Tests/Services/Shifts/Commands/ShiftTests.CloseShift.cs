using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Shifts;

public partial class ShiftTests
{
    [Fact]
    public async Task CloseShiftAsync_WhenShiftNotFound_ReturnsFalse()
    {
        // Arrange
        var dto = new UpdateShiftRequestDto
        {
            StartTime = DateTime.UtcNow.AddHours(-1),
            StopTime = DateTime.UtcNow,
            PartsMade = 10, BarsConsumed = 1
        };
        var ct = CancellationToken.None;

        MockRepository.Setup(r => r.GetByIdAsync(99, ct)).ReturnsAsync((Shift?)null);

        // Act
        var result = await ShiftService.CloseShiftAsync(99, operatorId: 1, dto, ct);

        // Assert
        Assert.False(result);
        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task CloseShiftAsync_WhenWrongOperator_ThrowsInvalidOperationException()
    {
        // Arrange
        var dto = new UpdateShiftRequestDto
        {
            StartTime = DateTime.UtcNow.AddHours(-1),
            StopTime = DateTime.UtcNow,
            PartsMade = 10, BarsConsumed = 1
        };
        var ct = CancellationToken.None;

        var shift = new Shift(jobId: 1, operatorId: 5, barsConsumed: 0, startTime: DateTime.UtcNow.AddHours(-1));
        MockRepository.Setup(r => r.GetByIdAsync(shift.Id, ct)).ReturnsAsync(shift);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ShiftService.CloseShiftAsync(shift.Id, operatorId: 99, dto, ct));

        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task CloseShiftAsync_WhenStopTimeIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        var dto = new UpdateShiftRequestDto
        {
            StartTime = DateTime.UtcNow.AddHours(-1),
            StopTime = null,
            PartsMade = 10, BarsConsumed = 1
        };
        var ct = CancellationToken.None;
        const int operatorId = 5;

        var shift = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 0, startTime: DateTime.UtcNow.AddHours(-1));
        MockRepository.Setup(r => r.GetByIdAsync(shift.Id, ct)).ReturnsAsync(shift);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ShiftService.CloseShiftAsync(shift.Id, operatorId, dto, ct));

        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task CloseShiftAsync_WhenStopTimeBeforeStartTime_ThrowsInvalidOperationException()
    {
        // Arrange
        var startTime = DateTime.UtcNow;
        var dto = new UpdateShiftRequestDto
        {
            StartTime = startTime,
            StopTime = startTime.AddHours(-1), // stop before start
            PartsMade = 10, BarsConsumed = 1
        };
        var ct = CancellationToken.None;
        const int operatorId = 5;

        var shift = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 0, startTime: startTime);
        MockRepository.Setup(r => r.GetByIdAsync(shift.Id, ct)).ReturnsAsync(shift);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ShiftService.CloseShiftAsync(shift.Id, operatorId, dto, ct));

        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task CloseShiftAsync_WhenValid_ClosesAndReturnsTrue()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(-2);
        var stopTime = DateTime.UtcNow;
        var dto = new UpdateShiftRequestDto
        {
            StartTime = startTime,
            StopTime = stopTime,
            PartsMade = 20, BarsConsumed = 4,
            PartsPerBar = 5
        };
        var ct = CancellationToken.None;
        const int operatorId = 5;

        var shift = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 0, startTime: startTime);
        MockRepository.Setup(r => r.GetByIdAsync(shift.Id, ct)).ReturnsAsync(shift);
        MockRepository.Setup(r => r.SaveChangesAsync(ct)).Returns(Task.CompletedTask);

        // Act
        var result = await ShiftService.CloseShiftAsync(shift.Id, operatorId, dto, ct);

        // Assert
        Assert.True(result);
        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Once);
    }
}
