using CncApp.Application.Dtos.CloseJob;
using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Common;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Workflows.CloseJob;

public partial class CloseJobTests
{
    private static Job CreateStartedJob(int id = 1)
    {
        var job = new Job(
            orderId: 1,
            stockLotId: 1,
            machineId: 1,
            partAmountPlanned: 10,
            barAmountPlanned: 5,
            barCycleTime: TimeSpan.FromMinutes(1),
            estimatedPartsPerBar: 5,
            dueDate: new DateOnly(2026, 6, 1))
        { Id = id };
        job.Start(1);
        return job;
    }

    private static CloseJobRequestDto CreateValidRequest(int shiftId = 10, int jobId = 1)
    {
        return new CloseJobRequestDto
        {
            ShiftId = shiftId,
            JobId = jobId,
            ShiftData = new UpdateShiftRequestDto
            {
                StartTime = new DateTime(2026, 4, 1, 8, 0, 0),
                StopTime = new DateTime(2026, 4, 1, 16, 0, 0),
                PartsMade = 50,
                BarsConsumed = 3,
                PartsPerBar = 17
            }
        };
    }

    [Fact]
    public async Task CloseJobAsync_WithValidData_ClosesShiftAndJobAndCommits()
    {
        // Arrange
        var dto = CreateValidRequest();
        var operatorId = 5;

        var shift = new Shift(
            jobId: dto.JobId,
            operatorId: operatorId,
            barsConsumed: 0,
            startTime: new DateTime(2026, 4, 1, 8, 0, 0))
        { Id = dto.ShiftId };

        var job = CreateStartedJob(dto.JobId);

        MockShiftRepository
            .Setup(r => r.GetByIdAsync(dto.ShiftId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shift);
        MockShiftRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockJobRepository
            .Setup(r => r.GetByIdAsync(dto.JobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);
        MockJobRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await Service.CloseJobAsync(dto, operatorId);

        // Assert
        Assert.Equal(dto.JobId, result.JobId);
        Assert.Equal(dto.ShiftId, result.ShiftId);
        Assert.NotEqual(default, result.JobEndedDateTime);
        Assert.NotNull(job.EndedDateTime);

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CloseJobAsync_WhenShiftNotFound_RollsBackAndThrows()
    {
        // Arrange
        var dto = CreateValidRequest();
        var operatorId = 5;

        MockShiftRepository
            .Setup(r => r.GetByIdAsync(dto.ShiftId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shift?)null);

        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => Service.CloseJobAsync(dto, operatorId));

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CloseJobAsync_WhenJobNotFound_RollsBackAndThrows()
    {
        // Arrange
        var dto = CreateValidRequest();
        var operatorId = 5;

        var shift = new Shift(
            jobId: dto.JobId,
            operatorId: operatorId,
            barsConsumed: 0,
            startTime: new DateTime(2026, 4, 1, 8, 0, 0))
        { Id = dto.ShiftId };

        MockShiftRepository
            .Setup(r => r.GetByIdAsync(dto.ShiftId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shift);
        MockShiftRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockJobRepository
            .Setup(r => r.GetByIdAsync(dto.JobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Job?)null);

        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => Service.CloseJobAsync(dto, operatorId));

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CloseJobAsync_WhenJobNotStarted_RollsBackAndThrows()
    {
        // Arrange
        var dto = CreateValidRequest();
        var operatorId = 5;

        var shift = new Shift(
            jobId: dto.JobId,
            operatorId: operatorId,
            barsConsumed: 0,
            startTime: new DateTime(2026, 4, 1, 8, 0, 0))
        { Id = dto.ShiftId };

        // Job that has NOT been started
        var job = new Job(
            orderId: 1,
            stockLotId: 1,
            machineId: 1,
            partAmountPlanned: 10,
            barAmountPlanned: 5,
            barCycleTime: TimeSpan.FromMinutes(1),
            estimatedPartsPerBar: 5,
            dueDate: new DateOnly(2026, 6, 1))
        { Id = dto.JobId };

        MockShiftRepository
            .Setup(r => r.GetByIdAsync(dto.ShiftId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shift);
        MockShiftRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockJobRepository
            .Setup(r => r.GetByIdAsync(dto.JobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);

        MockTransactionManager
            .Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockTransactionManager
            .Setup(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(
            () => Service.CloseJobAsync(dto, operatorId));

        MockTransactionManager.Verify(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        MockTransactionManager.Verify(t => t.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
