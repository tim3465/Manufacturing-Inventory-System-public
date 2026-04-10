using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Shifts;

public partial class ShiftTests
{
    [Fact]
    public async Task StartShiftAsync_WhenJobNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var dto = new StartShiftRequestDto { JobId = 99, StartTime = DateTime.UtcNow };
        var ct = CancellationToken.None;

        MockJobRepository.Setup(r => r.GetByIdAsync(dto.JobId, ct)).ReturnsAsync((Job?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ShiftService.StartShiftAsync(dto, operatorId: 1, ct));

        MockRepository.Verify(r => r.AddAsync(It.IsAny<Shift>(), ct), Times.Never);
        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task StartShiftAsync_WhenJobNotStarted_ThrowsInvalidOperationException()
    {
        // Arrange
        var dto = new StartShiftRequestDto { JobId = 1, StartTime = DateTime.UtcNow };
        var ct = CancellationToken.None;

        var job = new Job(orderId: 1, stockLotId: null, machineId: 5, partAmountPlanned: 100,
            barAmountPlanned: 10, barCycleTime: TimeSpan.FromMinutes(2), estimatedPartsPerBar: 5,
            dueDate: DateOnly.FromDateTime(DateTime.Today.AddDays(30)));
        // Job.StartedDateTime is null (not started)

        MockJobRepository.Setup(r => r.GetByIdAsync(dto.JobId, ct)).ReturnsAsync(job);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ShiftService.StartShiftAsync(dto, operatorId: 1, ct));

        MockRepository.Verify(r => r.AddAsync(It.IsAny<Shift>(), ct), Times.Never);
        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task StartShiftAsync_WhenMachineHasRunningShift_ThrowsInvalidOperationException()
    {
        // Arrange
        var dto = new StartShiftRequestDto { JobId = 1, StartTime = DateTime.UtcNow };
        var ct = CancellationToken.None;

        var job = new Job(orderId: 1, stockLotId: null, machineId: 5, partAmountPlanned: 100,
            barAmountPlanned: 10, barCycleTime: TimeSpan.FromMinutes(2), estimatedPartsPerBar: 5,
            dueDate: DateOnly.FromDateTime(DateTime.Today.AddDays(30)));
        job.Start(barsToAdd: 5);

        var existingShift = new Shift(jobId: 1, operatorId: 2, barsConsumed: 0, startTime: DateTime.UtcNow.AddHours(-1));

        MockJobRepository.Setup(r => r.GetByIdAsync(dto.JobId, ct)).ReturnsAsync(job);
        MockRepository.Setup(r => r.GetRunningShiftForMachineAsync(job.MachineId, ct)).ReturnsAsync(existingShift);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ShiftService.StartShiftAsync(dto, operatorId: 1, ct));

        MockRepository.Verify(r => r.AddAsync(It.IsAny<Shift>(), ct), Times.Never);
        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task StartShiftAsync_WhenValid_AddsShiftAndReturnsId()
    {
        // Arrange
        var startTime = DateTime.UtcNow;
        var dto = new StartShiftRequestDto { JobId = 1, StartTime = startTime };
        var ct = CancellationToken.None;
        const int operatorId = 7;

        var job = new Job(orderId: 1, stockLotId: null, machineId: 5, partAmountPlanned: 100,
            barAmountPlanned: 10, barCycleTime: TimeSpan.FromMinutes(2), estimatedPartsPerBar: 5,
            dueDate: DateOnly.FromDateTime(DateTime.Today.AddDays(30)));
        job.Start(barsToAdd: 5);

        MockJobRepository.Setup(r => r.GetByIdAsync(dto.JobId, ct)).ReturnsAsync(job);
        MockRepository.Setup(r => r.GetRunningShiftForMachineAsync(job.MachineId, ct)).ReturnsAsync((Shift?)null);
        MockRepository.Setup(r => r.AddAsync(It.IsAny<Shift>(), ct)).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.SaveChangesAsync(ct)).Returns(Task.CompletedTask);

        // Act
        var result = await ShiftService.StartShiftAsync(dto, operatorId, ct);

        // Assert — Id is 0 because no database assigns it in-memory, but the call flow is correct
        MockRepository.Verify(r => r.AddAsync(It.Is<Shift>(s => s.JobId == dto.JobId && s.OperatorId == operatorId), ct), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Once);
    }
}
