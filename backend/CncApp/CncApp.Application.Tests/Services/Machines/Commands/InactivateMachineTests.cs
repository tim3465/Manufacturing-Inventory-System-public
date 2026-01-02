using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Machines;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Machines.Commands;

public class InactivateMachineTests
{
    private readonly Mock<IMachineRepository> _mockRepository;
    private readonly Mock<AutoMapper.IMapper> _mockMapper;
    private readonly MachineService _machineService;

    public InactivateMachineTests()
    {
        _mockRepository = new Mock<IMachineRepository>();
        _mockMapper = new Mock<AutoMapper.IMapper>();
        _machineService = new MachineService(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task InactivateAsync_WhenMachineExists_ReturnsTrue()
    {
        // Arrange
        var machineId = 1;
        var inactivatedByUserId = 5;
        var cancellationToken = CancellationToken.None;

        _mockRepository
            .Setup(r => r.InactivateAsync(machineId, inactivatedByUserId, cancellationToken))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _machineService.InactivateAsync(machineId, inactivatedByUserId, cancellationToken);

        // Assert
        Assert.True(result);

        _mockRepository.Verify(r => r.InactivateAsync(machineId, inactivatedByUserId, cancellationToken), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InactivateAsync_WhenMachineDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var machineId = 999;
        int? inactivatedByUserId = null;
        var cancellationToken = CancellationToken.None;

        _mockRepository
            .Setup(r => r.InactivateAsync(machineId, inactivatedByUserId, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await _machineService.InactivateAsync(machineId, inactivatedByUserId, cancellationToken);

        // Assert
        Assert.False(result);

        _mockRepository.Verify(r => r.InactivateAsync(machineId, inactivatedByUserId, cancellationToken), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
    }
}

