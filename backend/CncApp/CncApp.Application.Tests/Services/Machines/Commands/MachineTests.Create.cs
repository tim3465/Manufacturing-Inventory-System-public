using AutoMapper;
using CncApp.Application.Dtos.Machines;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Machines;

public partial class MachineTests
{
    [Fact]
    public async Task CreateAsync_WhenValidDto_CreatesMachineAndReturnsId()
    {
        // Arrange
        var dto = new CreateMachineRequestDto
        {
            SerialNumber = "SN-TEST-001",
            ModelNumber = "MODEL-TEST-001"
        };
        var cancellationToken = CancellationToken.None;

        var machine = new Machine(dto.SerialNumber, dto.ModelNumber)
        {
            Id = 1
        };

        MockMapper
            .Setup(m => m.Map<Machine>(dto))
            .Returns(machine);

        MockRepository
            .Setup(r => r.AddAsync(It.IsAny<Machine>(), cancellationToken))
            .Returns(Task.CompletedTask);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await MachineService.CreateAsync(dto, cancellationToken);

        // Assert
        Assert.Equal(1, result);

        MockMapper.Verify(m => m.Map<Machine>(dto), Times.Once);
        MockRepository.Verify(r => r.AddAsync(It.IsAny<Machine>(), cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }
}

