using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Machines;

public partial class MachineTests
{
    [Fact]
    public async Task InactivateAsync_WhenMachineExists_ReturnsTrue()
    {
        // Arrange
        var machineId = 1;
        var inactivatedByUserId = 5;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.InactivateAsync(machineId, inactivatedByUserId, cancellationToken))
            .ReturnsAsync(true);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await MachineService.InactivateAsync(machineId, inactivatedByUserId, cancellationToken);

        // Assert
        Assert.True(result);

        MockRepository.Verify(r => r.InactivateAsync(machineId, inactivatedByUserId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InactivateAsync_WhenMachineDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var machineId = 999;
        int? inactivatedByUserId = null;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.InactivateAsync(machineId, inactivatedByUserId, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await MachineService.InactivateAsync(machineId, inactivatedByUserId, cancellationToken);

        // Assert
        Assert.False(result);

        MockRepository.Verify(r => r.InactivateAsync(machineId, inactivatedByUserId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
    }
}

