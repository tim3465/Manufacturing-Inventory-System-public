using CncApp.Application.Dtos.Users;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Users;

public partial class UserTests
{
    [Fact]
    public async Task UpdateRolesAsync_WhenUserExists_AssignsRoles()
    {
        var userId = 7;
        var ct = CancellationToken.None;
        var user = new User { Id = userId, IdentityUserId = 10 };
        var dto = new UpdateUserRolesRequestDto { Roles = new List<string> { "Admin", "User" } };

        MockRepository.Setup(r => r.GetByIdAsync(userId, ct)).ReturnsAsync(user);

        var result = await UserService.UpdateRolesAsync(userId, dto, ct);

        Assert.True(result);
        MockRepository.Verify(r => r.GetByIdAsync(userId, ct), Times.Once);
        MockIdentityProvisioningService.Verify(s => s.AssignRolesAsync(user.IdentityUserId, dto.Roles, ct), Times.Once);
    }

    [Fact]
    public async Task UpdateRolesAsync_WhenUserMissing_ReturnsFalse()
    {
        var userId = 8;
        var ct = CancellationToken.None;
        var dto = new UpdateUserRolesRequestDto { Roles = new List<string> { "User" } };

        MockRepository.Setup(r => r.GetByIdAsync(userId, ct)).ReturnsAsync((User?)null);

        var result = await UserService.UpdateRolesAsync(userId, dto, ct);

        Assert.False(result);
        MockRepository.Verify(r => r.GetByIdAsync(userId, ct), Times.Once);
        MockIdentityProvisioningService.Verify(s => s.AssignRolesAsync(It.IsAny<int>(), It.IsAny<IEnumerable<string>>(), ct), Times.Never);
    }
}

