using CncApp.Application.Dtos.Machines;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Machines;

public partial class MachineTests
{
    [Fact]
    public async Task ListActiveAsync_WhenMachinesExist_ReturnsListOfMachineDtos()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var machines = new List<Machine>
        {
            new Machine("SN-001", "MODEL-001") { Id = 1 },
            new Machine("SN-002", "MODEL-002") { Id = 2 }
        };

        var expectedDtos = new List<MachineDto>
        {
            new MachineDto { Id = 1, SerialNumber = "SN-001", ModelNumber = "MODEL-001" },
            new MachineDto { Id = 2, SerialNumber = "SN-002", ModelNumber = "MODEL-002" }
        };

        MockRepository
            .Setup(r => r.ListActiveAsync(cancellationToken))
            .ReturnsAsync(machines);

        MockMapper
            .Setup(m => m.Map<List<MachineDto>>(machines))
            .Returns(expectedDtos);

        // Act
        var result = await MachineService.ListActiveAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);

        MockRepository.Verify(r => r.ListActiveAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<MachineDto>>(machines), Times.Once);
    }

    [Fact]
    public async Task ListActiveAsync_WhenNoMachinesExist_ReturnsEmptyList()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var machines = new List<Machine>();

        MockRepository
            .Setup(r => r.ListActiveAsync(cancellationToken))
            .ReturnsAsync(machines);

        MockMapper
            .Setup(m => m.Map<List<MachineDto>>(machines))
            .Returns(new List<MachineDto>());

        // Act
        var result = await MachineService.ListActiveAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        MockRepository.Verify(r => r.ListActiveAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<MachineDto>>(machines), Times.Once);
    }
}

