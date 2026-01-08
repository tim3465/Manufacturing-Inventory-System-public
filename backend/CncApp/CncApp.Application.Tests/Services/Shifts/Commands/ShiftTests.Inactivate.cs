using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Shifts;

public partial class ShiftTests
{
    [Fact]
    public async Task InactivateAsync_WhenShiftExists_ReturnsTrueAndSaves()
    {
        var shiftId = 3;
        int? userId = 11;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.InactivateAsync(shiftId, userId, cancellationToken))
            .ReturnsAsync(true);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        var result = await ShiftService.InactivateAsync(shiftId, userId, cancellationToken);

        Assert.True(result);

        MockRepository.Verify(r => r.InactivateAsync(shiftId, userId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InactivateAsync_WhenShiftDoesNotExist_ReturnsFalseAndDoesNotSave()
    {
        var shiftId = 999;
        int? userId = null;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.InactivateAsync(shiftId, userId, cancellationToken))
            .ReturnsAsync(false);

        var result = await ShiftService.InactivateAsync(shiftId, userId, cancellationToken);

        Assert.False(result);

        MockRepository.Verify(r => r.InactivateAsync(shiftId, userId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
    }
}


