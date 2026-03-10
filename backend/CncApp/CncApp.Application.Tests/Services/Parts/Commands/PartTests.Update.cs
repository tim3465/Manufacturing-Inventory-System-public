using AutoMapper;
using CncApp.Application.Dtos.Parts;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Parts;

public partial class PartTests
{
    [Fact]
    public async Task UpdateAsync_WhenPartExists_UpdatesAndReturnsPartDto()
    {
        // Arrange
        var partId = 1;
        var dto = new UpdatePartRequestDto
        {
            ApproxPartCycleTime = TimeSpan.FromMinutes(10),
            CheckPerPart = 20
        };
        var cancellationToken = CancellationToken.None;

        var part = new Part("Test Part", "TP-001", TimeSpan.FromMinutes(5), 10)
        {
            Id = partId
        };

        var expectedDto = new PartDto
        {
            Id = partId,
            ApproxPartCycleTime = TimeSpan.FromMinutes(10),
            CheckPerPart = 20
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(partId, cancellationToken))
            .ReturnsAsync(part);

        MockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Part>(), cancellationToken))
            .Returns(Task.CompletedTask);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        MockMapper
            .Setup(m => m.Map<PartDto>(It.IsAny<Part>()))
            .Returns(expectedDto);

        // Act
        var result = await PartService.UpdateAsync(partId, dto, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(partId, result.Id);
        Assert.Equal(TimeSpan.FromMinutes(10), result.ApproxPartCycleTime);
        Assert.Equal(20, result.CheckPerPart);

        MockRepository.Verify(r => r.GetByIdAsync(partId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.UpdateAsync(It.IsAny<Part>(), cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<PartDto>(It.IsAny<Part>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenPartDoesNotExist_ReturnsNull()
    {
        // Arrange
        var partId = 999;
        var dto = new UpdatePartRequestDto
        {
            ApproxPartCycleTime = TimeSpan.FromMinutes(10)
        };
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(partId, cancellationToken))
            .ReturnsAsync((Part?)null);

        // Act
        var result = await PartService.UpdateAsync(partId, dto, cancellationToken);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(partId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.UpdateAsync(It.IsAny<Part>(), cancellationToken), Times.Never);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenOnlyApproxPartCycleTimeProvided_UpdatesOnlyThatField()
    {
        // Arrange
        var partId = 1;
        var dto = new UpdatePartRequestDto
        {
            ApproxPartCycleTime = TimeSpan.FromMinutes(15)
        };
        var cancellationToken = CancellationToken.None;

        var part = new Part("Test Part", "TP-001", TimeSpan.FromMinutes(5), 10)
        {
            Id = partId
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(partId, cancellationToken))
            .ReturnsAsync(part);

        MockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Part>(), cancellationToken))
            .Returns(Task.CompletedTask);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        MockMapper
            .Setup(m => m.Map<PartDto>(It.IsAny<Part>()))
            .Returns(new PartDto { Id = partId, ApproxPartCycleTime = TimeSpan.FromMinutes(15), CheckPerPart = 10 });

        // Act
        var result = await PartService.UpdateAsync(partId, dto, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TimeSpan.FromMinutes(15), part.ApproxPartCycleTime);
        Assert.Equal(10, part.CheckPerPart); // Original value unchanged
    }
}

