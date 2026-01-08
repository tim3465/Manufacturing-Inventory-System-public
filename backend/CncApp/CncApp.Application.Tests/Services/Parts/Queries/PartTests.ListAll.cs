using AutoMapper;
using CncApp.Application.Dtos.Parts;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Parts;

public partial class PartTests
{
    [Fact]
    public async Task ListAllAsync_WhenPartsExist_ReturnsListOfPartDtos()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var parts = new List<Part>
        {
            new Part(TimeSpan.FromMinutes(5), 10) { Id = 1 },
            new Part(TimeSpan.FromMinutes(10), 20) { Id = 2 }
        };

        var expectedDtos = new List<PartDto>
        {
            new PartDto { Id = 1, ApproxPartCycleTime = TimeSpan.FromMinutes(5), CheckPerPart = 10 },
            new PartDto { Id = 2, ApproxPartCycleTime = TimeSpan.FromMinutes(10), CheckPerPart = 20 }
        };

        MockRepository
            .Setup(r => r.ListAllAsync(cancellationToken))
            .ReturnsAsync(parts);

        MockMapper
            .Setup(m => m.Map<List<PartDto>>(parts))
            .Returns(expectedDtos);

        // Act
        var result = await PartService.ListAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);

        MockRepository.Verify(r => r.ListAllAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<PartDto>>(parts), Times.Once);
    }

    [Fact]
    public async Task ListAllAsync_WhenNoPartsExist_ReturnsEmptyList()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var parts = new List<Part>();

        MockRepository
            .Setup(r => r.ListAllAsync(cancellationToken))
            .ReturnsAsync(parts);

        MockMapper
            .Setup(m => m.Map<List<PartDto>>(parts))
            .Returns(new List<PartDto>());

        // Act
        var result = await PartService.ListAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        MockRepository.Verify(r => r.ListAllAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<PartDto>>(parts), Times.Once);
    }
}

