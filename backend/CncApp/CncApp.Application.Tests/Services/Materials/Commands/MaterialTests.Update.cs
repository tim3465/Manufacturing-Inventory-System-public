using AutoMapper;
using CncApp.Application.Dtos.Materials;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Materials;

public partial class MaterialTests
{
    [Fact]
    public async Task UpdateAsync_WhenMaterialExists_UpdatesAndReturnsMaterialDto()
    {
        // Arrange
        var materialId = 1;
        var dto = new UpdateMaterialRequestDto
        {
            HeatNumber = "HN999999",
            MaterialName = "Steel-B2"
        };
        var cancellationToken = CancellationToken.None;

        var material = new Material("HN123456", "Steel-A1")
        {
            Id = materialId
        };

        var expectedDto = new MaterialDto
        {
            Id = materialId,
            HeatNumber = "HN999999",
            MaterialName = "Steel-B2"
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(materialId, cancellationToken))
            .ReturnsAsync(material);

        MockMapper
            .Setup(m => m.Map(dto, material))
            .Verifiable();

        MockMapper
            .Setup(m => m.Map<MaterialDto>(material))
            .Returns(expectedDto);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await MaterialService.UpdateAsync(materialId, dto, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(materialId, result.Id);
        Assert.Equal("HN999999", result.HeatNumber);
        Assert.Equal("Steel-B2", result.MaterialName);

        MockRepository.Verify(r => r.GetByIdAsync(materialId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map(dto, material), Times.Once);
        MockMapper.Verify(m => m.Map<MaterialDto>(material), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenMaterialDoesNotExist_ReturnsNull()
    {
        // Arrange
        var materialId = 999;
        var dto = new UpdateMaterialRequestDto
        {
            HeatNumber = "HN999999",
            MaterialName = "Steel-B2"
        };
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(materialId, cancellationToken))
            .ReturnsAsync((Material?)null);

        // Act
        var result = await MaterialService.UpdateAsync(materialId, dto, cancellationToken);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(materialId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map(It.IsAny<UpdateMaterialRequestDto>(), It.IsAny<Material>()), Times.Never);
        MockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

