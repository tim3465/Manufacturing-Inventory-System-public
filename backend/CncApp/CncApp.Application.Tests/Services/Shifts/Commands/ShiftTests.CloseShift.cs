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
        MockJobRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>(), ct), Times.Never);
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
        MockJobRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>(), ct), Times.Never);
    }

    [Fact]
    public async Task CloseShiftAsync_WhenJobNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var dto = new UpdateShiftRequestDto
        {
            StartTime = DateTime.UtcNow.AddHours(-1),
            PartsMade = 10, BarsConsumed = 1
        };
        var ct = CancellationToken.None;
        const int operatorId = 5;

        var shift = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 0, startTime: DateTime.UtcNow.AddHours(-1));
        MockRepository.Setup(r => r.GetByIdAsync(shift.Id, ct)).ReturnsAsync(shift);
        MockJobRepository.Setup(r => r.GetByIdAsync(shift.JobId, ct)).ReturnsAsync((Job?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ShiftService.CloseShiftAsync(shift.Id, operatorId, dto, ct));

        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task CloseShiftAsync_WhenValid_ClosesShiftAndEndsJobAndReturnsTrue()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(-2);
        var dto = new UpdateShiftRequestDto
        {
            StartTime = startTime,
            PartsMade = 20, BarsConsumed = 4,
            PartsPerBar = 5
        };
        var ct = CancellationToken.None;
        const int operatorId = 5;

        var shift = new Shift(jobId: 1, operatorId: operatorId, barsConsumed: 0, startTime: startTime);
        var job = new Job(
            orderId: 1, stockLotId: null, machineId: 1,
            partAmountPlanned: 100, barAmountPlanned: 10,
            barCycleTime: TimeSpan.FromMinutes(5),
            estimatedPartsPerBar: 10, dueDate: DateOnly.FromDateTime(DateTime.Today.AddDays(7)));

        MockRepository.Setup(r => r.GetByIdAsync(shift.Id, ct)).ReturnsAsync(shift);
        MockJobRepository.Setup(r => r.GetByIdAsync(shift.JobId, ct)).ReturnsAsync(job);
        MockRepository.Setup(r => r.SaveChangesAsync(ct)).Returns(Task.CompletedTask);

        var before = DateTime.UtcNow;

        // Act
        var result = await ShiftService.CloseShiftAsync(shift.Id, operatorId, dto, ct);

        var after = DateTime.UtcNow;

        // Assert
        Assert.True(result);
        Assert.NotNull(shift.StopTime);
        Assert.True(shift.StopTime >= before && shift.StopTime <= after);
        Assert.NotNull(job.EndedDateTime);
        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Once);
    }
}
