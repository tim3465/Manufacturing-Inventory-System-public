using AutoMapper;
using CncApp.Application.Dtos.Machines;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Machines;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Machines.Queries;

public class GetMachineTests
{
    private readonly Mock<IMachineRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MachineService _machineService;

    public GetMachineTests()
    {
        _mockRepository = new Mock<IMachineRepository>();
        _mockMapper = new Mock<IMapper>();
        _machineService = new MachineService(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAsync_WhenMachineExists_ReturnsMachineDto()
    {
        // Arrange
        var machineId = 1;
        var cancellationToken = CancellationToken.None;

        var machine = new Machine
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

        _mockRepository
            .Setup(r => r.GetByIdAsync(machineId, cancellationToken))
            .ReturnsAsync(machine);

        _mockMapper
            .Setup(m => m.Map<MachineDto>(machine))
            .Returns(expectedDto);

        // Act
        var result = await _machineService.GetAsync(machineId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(machineId, result.Id);
        Assert.Equal("SN-12345", result.SerialNumber);
        Assert.Equal("MODEL-001", result.ModelNumber);

        _mockRepository.Verify(r => r.GetByIdAsync(machineId, cancellationToken), Times.Once);
        _mockMapper.Verify(m => m.Map<MachineDto>(machine), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenMachineDoesNotExist_ReturnsNull()
    {
        // Arrange
        var machineId = 999;
        var cancellationToken = CancellationToken.None;

        _mockRepository
            .Setup(r => r.GetByIdAsync(machineId, cancellationToken))
            .ReturnsAsync((Machine?)null);

        // Act
        var result = await _machineService.GetAsync(machineId, cancellationToken);

        // Assert
        Assert.Null(result);

        _mockRepository.Verify(r => r.GetByIdAsync(machineId, cancellationToken), Times.Once);
        _mockMapper.Verify(m => m.Map<MachineDto>(It.IsAny<Machine>()), Times.Never());
    }
}
