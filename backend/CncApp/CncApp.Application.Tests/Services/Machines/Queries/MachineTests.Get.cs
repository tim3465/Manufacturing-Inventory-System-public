using AutoMapper;
using CncApp.Application.Dtos.Machines;
using CncApp.Domain.Entities;
using Moq;

using Xunit;

namespace CncApp.Application.Tests.Services.Machines;
public partial class MachineTests
{
    [Fact]
    public async Task GetAsync_WhenMachineExists_ReturnsMachineDto()
    {
        // Arrange
        var machineId = 1;
        var cancellationToken = CancellationToken.None;

        var machine = new Machine("SN-TEST-001", "MODEL-TEST-001")
        {
            Id = machineId,
            SerialNumber = "SN-12345",
            ModelNumber = "MODEL-001"
        };

        var expectedDto = new MachineDto
        {
            Id = machineId,
            SerialNumber = "SN-12345",
            ModelNumber = "MODEL-001"
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(machineId, cancellationToken))
            .ReturnsAsync(machine);

        MockMapper
            .Setup(m => m.Map<MachineDto>(machine))
            .Returns(expectedDto);

        // Act
        var result = await MachineService.GetAsync(machineId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(machineId, result.Id);
        Assert.Equal("SN-12345", result.SerialNumber);
        Assert.Equal("MODEL-001", result.ModelNumber);

        MockRepository.Verify(r => r.GetByIdAsync(machineId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<MachineDto>(machine), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenMachineDoesNotExist_ReturnsNull()
    {
        // Arrange
        var machineId = 999;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(machineId, cancellationToken))
            .ReturnsAsync((Machine?)null);

        // Act
        var result = await MachineService.GetAsync(machineId, cancellationToken);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(machineId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<MachineDto>(It.IsAny<Machine>()), Times.Never());
    }
}

