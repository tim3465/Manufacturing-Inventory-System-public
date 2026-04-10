using CncApp.Application.Dtos.Jobs;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Jobs;

public partial class JobTests
{
    [Fact]
    public async Task GetJobShiftsForOperatorAsync_WhenJobFoundAndAuthorized_ReturnsMappedShiftDtos()
    {
        // Arrange
        var jobId = 1;
        var operatorId = 5;
        var ct = CancellationToken.None;

        var shift1 = new Shift(jobId, operatorId, 2, DateTime.UtcNow) { Id = 10 };
        var shift2 = new Shift(jobId, operatorId, 1, DateTime.UtcNow) { Id = 11 };

        var job = new Job(1, null, 2, 100, 10, TimeSpan.FromMinutes(1), 5, new DateOnly(2026, 6, 1))
        {
            Id = jobId
        };
        job.Shifts.Add(shift1);
        job.Shifts.Add(shift2);

        var expectedDtos = new List<JobShiftDto>
        {
            new JobShiftDto { ShiftId = 10, MachinistName = "John Doe", StartDateTime = shift1.StartTime },
            new JobShiftDto { ShiftId = 11, MachinistName = "John Doe", StartDateTime = shift2.StartTime }
        };

        MockRepository
            .Setup(r => r.GetWithShiftsByIdForOperatorAsync(jobId, operatorId, ct))
            .ReturnsAsync(job);

        MockMapper
            .Setup(m => m.Map<List<JobShiftDto>>(job.Shifts))
            .Returns(expectedDtos);

        // Act
        var result = await JobService.GetJobShiftsForOperatorAsync(jobId, operatorId, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(10, result[0].ShiftId);
        Assert.Equal(11, result[1].ShiftId);

        MockRepository.Verify(r => r.GetWithShiftsByIdForOperatorAsync(jobId, operatorId, ct), Times.Once);
        MockMapper.Verify(m => m.Map<List<JobShiftDto>>(job.Shifts), Times.Once);
    }

    [Fact]
    public async Task GetJobShiftsForOperatorAsync_WhenJobNotFoundOrUnauthorized_ReturnsNull()
    {
        // Arrange
        var jobId = 999;
        var operatorId = 5;
        var ct = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetWithShiftsByIdForOperatorAsync(jobId, operatorId, ct))
            .ReturnsAsync((Job?)null);

        // Act
        var result = await JobService.GetJobShiftsForOperatorAsync(jobId, operatorId, ct);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetWithShiftsByIdForOperatorAsync(jobId, operatorId, ct), Times.Once);
        MockMapper.Verify(m => m.Map<List<JobShiftDto>>(It.IsAny<object>()), Times.Never);
    }
}
