using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Materials;

public partial class MaterialTests
{
    [Fact]
    public async Task InactivateAsync_WhenMaterialExists_ReturnsTrue()
    {
        // Arrange
        var materialId = 1;
        var inactivatedByUserId = 5;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.InactivateAsync(materialId, inactivatedByUserId, cancellationToken))
            .ReturnsAsync(true);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await MaterialService.InactivateAsync(materialId, inactivatedByUserId, cancellationToken);

        // Assert
        Assert.True(result);

        MockRepository.Verify(r => r.InactivateAsync(materialId, inactivatedByUserId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InactivateAsync_WhenMaterialDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var materialId = 999;
        int? inactivatedByUserId = null;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.InactivateAsync(materialId, inactivatedByUserId, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await MaterialService.InactivateAsync(materialId, inactivatedByUserId, cancellationToken);

        // Assert
        Assert.False(result);

        MockRepository.Verify(r => r.InactivateAsync(materialId, inactivatedByUserId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
    }
}

