using AutoMapper;
using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Shifts;

public partial class ShiftTests
{
    [Fact]
    public async Task CreateAsync_WhenValidDto_AddsShiftAndReturnsId()
    {
        // Arrange
        var dto = new CreateShiftRequestDto
        {
            JobId = 10,
            OperatorId = 20,
            BarsConsumed = 1,
            PartsMade = 5,
            Scrap = 1,
            PartsPerBar = 2,
            StartTime = DateTime.UtcNow
        };

        var ct = CancellationToken.None;

        var shift = new Shift(10, 20, 1, dto.StartTime) { Id = 7 };

        MockMapper.Setup(m => m.Map<Shift>(dto)).Returns(shift);

        // optional: enforce call order
        var seq = new MockSequence();
        MockRepository.InSequence(seq).Setup(r => r.AddAsync(shift, ct)).Returns(Task.CompletedTask);
        MockRepository.InSequence(seq).Setup(r => r.SaveChangesAsync(ct)).Returns(Task.CompletedTask);

        // Act
        var result = await ShiftService.CreateAsync(dto, ct);

        // Assert
        Assert.Equal(7, result);

        MockMapper.Verify(m => m.Map<Shift>(dto), Times.Once);
        MockRepository.Verify(r => r.AddAsync(shift, ct), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Once);
    }

}


