using CncApp.Application.Dtos.Users;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Users;

public partial class UserTests
{
    [Fact]
    public async Task ListActiveAsync_ReturnsMappedUsers()
    {
        var ct = CancellationToken.None;
        var users = new List<User>
        {
            new() { Id = 1, UserName = "a" },
            new() { Id = 2, UserName = "b" }
        };
        var dtos = new List<UserDto>
        {
            new() { Id = 1, UserName = "a" },
            new() { Id = 2, UserName = "b" }
        };

        MockRepository.Setup(r => r.ListActiveAsync(ct)).ReturnsAsync(users);
        MockMapper.Setup(m => m.Map<List<UserDto>>(users)).Returns(dtos);

        var result = await UserService.ListActiveAsync(ct);

        Assert.Equal(2, result.Count);
        MockRepository.Verify(r => r.ListActiveAsync(ct), Times.Once);
        MockMapper.Verify(m => m.Map<List<UserDto>>(users), Times.Once);
    }
}

