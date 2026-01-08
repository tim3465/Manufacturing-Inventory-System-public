using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Users;

public partial class UserTests
{
    [Fact]
    public async Task GetCurrentUserAsync_WhenDomainUserExists_ReturnsUser()
    {
        // Arrange
        var identityUserId = 42;
        var cancellationToken = CancellationToken.None;

        var user = new User
        {
            IdentityUserId = identityUserId,
            Id = 7,
            UserName = "user@test.local"
        };

        MockCurrentUserService
            .Setup(s => s.GetCurrentUserId())
            .Returns(identityUserId);

        MockRepository
            .Setup(r => r.GetByIdentityUserIdAsync(identityUserId, cancellationToken))
            .ReturnsAsync(user);

        // Act
        var result = await UserService.GetCurrentUserAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Id);
        Assert.Equal(identityUserId, result.IdentityUserId);

        MockCurrentUserService.Verify(s => s.GetCurrentUserId(), Times.Once);
        MockRepository.Verify(r => r.GetByIdentityUserIdAsync(identityUserId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WhenDomainUserMissing_ThrowsInvalidOperationException()
    {
        // Arrange
        var identityUserId = 99;
        var cancellationToken = CancellationToken.None;

        MockCurrentUserService
            .Setup(s => s.GetCurrentUserId())
            .Returns(identityUserId);

        MockRepository
            .Setup(r => r.GetByIdentityUserIdAsync(identityUserId, cancellationToken))
            .ReturnsAsync((User?)null);

        // Act / Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => UserService.GetCurrentUserAsync(cancellationToken));

        MockCurrentUserService.Verify(s => s.GetCurrentUserId(), Times.Once);
        MockRepository.Verify(r => r.GetByIdentityUserIdAsync(identityUserId, cancellationToken), Times.Once);
    }
}

