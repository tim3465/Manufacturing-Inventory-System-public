using CncApp.Application.Dtos.Users;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Users;

public partial class UserTests
{
    [Fact]
    public async Task GetAsync_WhenUserExists_ReturnsDto()
    {
        var userId = 1;
        var ct = CancellationToken.None;
        var user = new User { Id = userId, UserName = "user@test" };
        var dto = new UserDto { Id = userId, UserName = "user@test" };

        MockRepository.Setup(r => r.GetByIdAsync(userId, ct)).ReturnsAsync(user);
        MockMapper.Setup(m => m.Map<UserDto>(user)).Returns(dto);

        var result = await UserService.GetAsync(userId, ct);

        Assert.NotNull(result);
        Assert.Equal(userId, result!.Id);

        MockRepository.Verify(r => r.GetByIdAsync(userId, ct), Times.Once);
        MockMapper.Verify(m => m.Map<UserDto>(user), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenUserMissing_ReturnsNull()
    {
        var userId = 99;
        var ct = CancellationToken.None;

        MockRepository.Setup(r => r.GetByIdAsync(userId, ct)).ReturnsAsync((User?)null);

        var result = await UserService.GetAsync(userId, ct);

        Assert.Null(result);
        MockRepository.Verify(r => r.GetByIdAsync(userId, ct), Times.Once);
        MockMapper.Verify(m => m.Map<UserDto>(It.IsAny<User>()), Times.Never);
    }
}

