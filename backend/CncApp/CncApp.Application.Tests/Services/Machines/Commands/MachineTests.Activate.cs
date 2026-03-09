using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Machines;

public partial class MachineTests
{
    [Fact]
    public async Task ActivateAsync_WhenMachineExists_ReturnsTrue()
    {
        // Arrange
        var machineId = 1;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.ActivateAsync(machineId, cancellationToken))
            .ReturnsAsync(true);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await MachineService.ActivateAsync(machineId, cancellationToken);

        // Assert
        Assert.True(result);

        MockRepository.Verify(r => r.ActivateAsync(machineId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ActivateAsync_WhenMachineDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var machineId = 999;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.ActivateAsync(machineId, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await MachineService.ActivateAsync(machineId, cancellationToken);

        // Assert
        Assert.False(result);

        MockRepository.Verify(r => r.ActivateAsync(machineId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
    }
}
