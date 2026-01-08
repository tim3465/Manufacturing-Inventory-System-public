using AutoMapper;
using CncApp.Application.Dtos.Parts;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Parts;

public partial class PartTests
{
    [Fact]
    public async Task GetAsync_WhenPartExists_ReturnsPartDto()
    {
        // Arrange
        var partId = 1;
        var cancellationToken = CancellationToken.None;

        var part = new Part(TimeSpan.FromMinutes(5), 10)
        {
            Id = partId
        };

        var expectedDto = new PartDto
        {
            Id = partId,
            ApproxPartCycleTime = TimeSpan.FromMinutes(5),
            CheckPerPart = 10
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(partId, cancellationToken))
            .ReturnsAsync(part);

        MockMapper
            .Setup(m => m.Map<PartDto>(part))
            .Returns(expectedDto);

        // Act
        var result = await PartService.GetAsync(partId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(partId, result.Id);
        Assert.Equal(TimeSpan.FromMinutes(5), result.ApproxPartCycleTime);
        Assert.Equal(10, result.CheckPerPart);

        MockRepository.Verify(r => r.GetByIdAsync(partId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<PartDto>(part), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenPartDoesNotExist_ReturnsNull()
    {
        // Arrange
        var partId = 999;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(partId, cancellationToken))
            .ReturnsAsync((Part?)null);

        // Act
        var result = await PartService.GetAsync(partId, cancellationToken);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(partId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<PartDto>(It.IsAny<Part>()), Times.Never);
    }
}

