using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Users;

public partial class UserTests
{
    [Fact]
    public async Task GetRolesAsync_WhenUserExists_ReturnsDtoWithRoles()
    {
        var userId = 1;
        var identityUserId = 10;
        var ct = CancellationToken.None;
        var user = new User { Id = userId, IdentityUserId = identityUserId, UserName = "user@test" };
        var expectedRoles = new List<string> { "Admin", "User" };

        MockRepository.Setup(r => r.GetByIdAsync(userId, ct)).ReturnsAsync(user);
        MockIdentityProvisioningService.Setup(s => s.GetRolesAsync(identityUserId, ct)).ReturnsAsync(expectedRoles);

        var result = await UserService.GetRolesAsync(userId, ct);

        Assert.NotNull(result);
        Assert.Equal(userId, result!.UserId);
        Assert.Equal(expectedRoles, result.Roles);

        MockRepository.Verify(r => r.GetByIdAsync(userId, ct), Times.Once);
        MockIdentityProvisioningService.Verify(s => s.GetRolesAsync(identityUserId, ct), Times.Once);
    }

    [Fact]
    public async Task GetRolesAsync_WhenUserMissing_ReturnsNull()
    {
        var userId = 99;
        var ct = CancellationToken.None;

        MockRepository.Setup(r => r.GetByIdAsync(userId, ct)).ReturnsAsync((User?)null);

        var result = await UserService.GetRolesAsync(userId, ct);

        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(userId, ct), Times.Once);
        MockIdentityProvisioningService.Verify(s => s.GetRolesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

