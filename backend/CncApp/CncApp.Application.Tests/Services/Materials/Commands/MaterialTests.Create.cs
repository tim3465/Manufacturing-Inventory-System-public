using AutoMapper;
using CncApp.Application.Dtos.Materials;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Materials;

public partial class MaterialTests
{
    [Fact]
    public async Task CreateAsync_WhenValidDto_CreatesMaterialAndReturnsId()
    {
        // Arrange
        var dto = new CreateMaterialRequestDto
        {
            HeatNumber = "HN123456",
            MaterialName = "Steel-A1"
        };
        var cancellationToken = CancellationToken.None;

        var material = new Material(dto.HeatNumber, dto.MaterialName)
        {
            Id = 1
        };

        MockMapper
            .Setup(m => m.Map<Material>(dto))
            .Returns(material);

        MockRepository
            .Setup(r => r.AddAsync(It.IsAny<Material>(), cancellationToken))
            .Returns(Task.CompletedTask);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await MaterialService.CreateAsync(dto, cancellationToken);

        // Assert
        Assert.Equal(1, result);

        MockMapper.Verify(m => m.Map<Material>(dto), Times.Once);
        MockRepository.Verify(r => r.AddAsync(It.IsAny<Material>(), cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }
}

