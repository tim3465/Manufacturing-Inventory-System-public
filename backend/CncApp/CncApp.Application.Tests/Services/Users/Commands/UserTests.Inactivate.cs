using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Users;

public partial class UserTests
{
    [Fact]
    public async Task InactivateAsync_WhenFound_ReturnsTrueAndSaves()
    {
        var userId = 5;
        var ct = CancellationToken.None;

        MockRepository.Setup(r => r.InactivateAsync(userId, null, ct)).ReturnsAsync(true);

        var result = await UserService.InactivateAsync(userId, null, ct);

        Assert.True(result);
        MockRepository.Verify(r => r.InactivateAsync(userId, null, ct), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Once);
    }

    [Fact]
    public async Task InactivateAsync_WhenMissing_ReturnsFalse()
    {
        var userId = 6;
        var ct = CancellationToken.None;

        MockRepository.Setup(r => r.InactivateAsync(userId, null, ct)).ReturnsAsync(false);

        var result = await UserService.InactivateAsync(userId, null, ct);

        Assert.False(result);
        MockRepository.Verify(r => r.InactivateAsync(userId, null, ct), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(ct), Times.Never);
    }
}

