using AutoMapper;
using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Shifts;

public partial class ShiftTests
{
    [Fact]
    public async Task GetAsync_WhenShiftExists_ReturnsShiftDto()
    {
        var shiftId = 1;
        var cancellationToken = CancellationToken.None;

        var shift = new Shift(jobId: 10, operatorId: 20, barsConsumed: 1, startTime: DateTime.UtcNow)
        {
            Id = shiftId,
            PartsMade = 5,
            Scrap = 1,
            PartsPerBar = 2
        };

        var expectedDto = new ShiftDto
        {
            Id = shiftId,
            JobId = shift.JobId,
            OperatorId = shift.OperatorId,
            BarsConsumed = shift.BarsConsumed,
            PartsMade = shift.PartsMade,
            Scrap = shift.Scrap,
            PartsPerBar = shift.PartsPerBar,
            StartTime = shift.StartTime,
            StopTime = shift.StopTime,
            Downtime = shift.Downtime
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(shiftId, cancellationToken))
            .ReturnsAsync(shift);

        MockMapper
            .Setup(m => m.Map<ShiftDto>(shift))
            .Returns(expectedDto);

        var result = await ShiftService.GetAsync(shiftId, cancellationToken);

        Assert.NotNull(result);
        Assert.Equal(expectedDto.Id, result!.Id);
        Assert.Equal(expectedDto.JobId, result.JobId);
        Assert.Equal(expectedDto.OperatorId, result.OperatorId);

        MockRepository.Verify(r => r.GetByIdAsync(shiftId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<ShiftDto>(shift), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenShiftDoesNotExist_ReturnsNull()
    {
        var shiftId = 999;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(shiftId, cancellationToken))
            .ReturnsAsync((Shift?)null);

        var result = await ShiftService.GetAsync(shiftId, cancellationToken);

        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(shiftId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<ShiftDto>(It.IsAny<Shift>()), Times.Never);
    }
}


