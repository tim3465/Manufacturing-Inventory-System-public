using AutoMapper;
using CncApp.Application.Dtos.Materials;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Materials;

public partial class MaterialTests
{
    [Fact]
    public async Task GetAsync_WhenMaterialExists_ReturnsMaterialDto()
    {
        // Arrange
        var materialId = 1;
        var cancellationToken = CancellationToken.None;

        var material = new Material("HN123456", "Steel-A1")
        {
            Id = materialId,
            HeatNumber = "HN123456",
            MaterialName = "Steel-A1"
        };

        var expectedDto = new MaterialDto
        {
            Id = materialId,
            HeatNumber = "HN123456",
            MaterialName = "Steel-A1"
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(materialId, cancellationToken))
            .ReturnsAsync(material);

        MockMapper
            .Setup(m => m.Map<MaterialDto>(material))
            .Returns(expectedDto);

        // Act
        var result = await MaterialService.GetAsync(materialId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(materialId, result.Id);
        Assert.Equal("HN123456", result.HeatNumber);
        Assert.Equal("Steel-A1", result.MaterialName);

        MockRepository.Verify(r => r.GetByIdAsync(materialId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<MaterialDto>(material), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenMaterialDoesNotExist_ReturnsNull()
    {
        // Arrange
        var materialId = 999;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(materialId, cancellationToken))
            .ReturnsAsync((Material?)null);

        // Act
        var result = await MaterialService.GetAsync(materialId, cancellationToken);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(materialId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<MaterialDto>(It.IsAny<Material>()), Times.Never());
    }
}

