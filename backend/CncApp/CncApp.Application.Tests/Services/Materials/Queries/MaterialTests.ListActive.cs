using AutoMapper;
using CncApp.Application.Dtos.Materials;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Materials;

public partial class MaterialTests
{
    [Fact]
    public async Task ListActiveAsync_WhenMaterialsExist_ReturnsListOfMaterialDtos()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var materials = new List<Material>
        {
            new Material("HN123456", "Steel-A1") { Id = 1 },
            new Material("HN789012", "Steel-B2") { Id = 2 }
        };

        var expectedDtos = new List<MaterialDto>
        {
            new MaterialDto { Id = 1, HeatNumber = "HN123456", MaterialName = "Steel-A1" },
            new MaterialDto { Id = 2, HeatNumber = "HN789012", MaterialName = "Steel-B2" }
        };

        MockRepository
            .Setup(r => r.ListActiveAsync(cancellationToken))
            .ReturnsAsync(materials);

        MockMapper
            .Setup(m => m.Map<List<MaterialDto>>(materials))
            .Returns(expectedDtos);

        // Act
        var result = await MaterialService.ListActiveAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);

        MockRepository.Verify(r => r.ListActiveAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<MaterialDto>>(materials), Times.Once);
    }

    [Fact]
    public async Task ListActiveAsync_WhenNoMaterialsExist_ReturnsEmptyList()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var materials = new List<Material>();
        var expectedDtos = new List<MaterialDto>();

        MockRepository
            .Setup(r => r.ListActiveAsync(cancellationToken))
            .ReturnsAsync(materials);

        MockMapper
            .Setup(m => m.Map<List<MaterialDto>>(materials))
            .Returns(expectedDtos);

        // Act
        var result = await MaterialService.ListActiveAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        MockRepository.Verify(r => r.ListActiveAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<MaterialDto>>(materials), Times.Once);
    }
}

