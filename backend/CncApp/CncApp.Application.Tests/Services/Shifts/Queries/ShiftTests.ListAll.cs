using AutoMapper;
using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Shifts;

public partial class ShiftTests
{
    [Fact]
    public async Task ListAllAsync_WhenShiftsExist_ReturnsDtos()
    {
        var cancellationToken = CancellationToken.None;

        var shifts = new List<Shift>
        {
            new Shift(1, 2, 1, DateTime.UtcNow) { Id = 1, PartsMade = 5, Scrap = 0 },
            new Shift(3, 4, 2, DateTime.UtcNow) { Id = 2, PartsMade = 10, Scrap = 1 }
        };

        var expectedDtos = new List<ShiftDto>
        {
            new ShiftDto { Id = 1, JobId = 1, OperatorId = 2, BarsConsumed = 1, PartsMade = 5, Scrap = 0 },
            new ShiftDto { Id = 2, JobId = 3, OperatorId = 4, BarsConsumed = 2, PartsMade = 10, Scrap = 1 }
        };

        MockRepository
            .Setup(r => r.ListAllAsync(cancellationToken))
            .ReturnsAsync(shifts);

        MockMapper
            .Setup(m => m.Map<List<ShiftDto>>(shifts))
            .Returns(expectedDtos);

        var result = await ShiftService.ListAllAsync(cancellationToken);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(expectedDtos[0].Id, result[0].Id);
        Assert.Equal(expectedDtos[1].Id, result[1].Id);

        MockRepository.Verify(r => r.ListAllAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<ShiftDto>>(shifts), Times.Once);
    }

    [Fact]
    public async Task ListAllAsync_WhenNoShiftsExist_ReturnsEmptyList()
    {
        var cancellationToken = CancellationToken.None;
        var shifts = new List<Shift>();

        MockRepository
            .Setup(r => r.ListAllAsync(cancellationToken))
            .ReturnsAsync(shifts);

        MockMapper
            .Setup(m => m.Map<List<ShiftDto>>(shifts))
            .Returns(new List<ShiftDto>());

        var result = await ShiftService.ListAllAsync(cancellationToken);

        Assert.NotNull(result);
        Assert.Empty(result);

        MockRepository.Verify(r => r.ListAllAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<ShiftDto>>(shifts), Times.Once);
    }
}


