using System.Security.Claims;
using CncApp.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Moq;

namespace CncApp.Application.Tests.Services.Auth;

public class CurrentUserServiceTests
{
    // Use the literal scheme name so we don't need a package ref for JwtBearerDefaults
    private const string JwtBearerScheme = "Bearer";

    private static CurrentUserService CreateServiceWithContext(HttpContext? httpContext)
    {
        var mockAccessor = new Mock<IHttpContextAccessor>();
        mockAccessor.Setup(a => a.HttpContext).Returns(httpContext);
        return new CurrentUserService(mockAccessor.Object);
    }

    private static ClaimsPrincipal AuthenticatedPrincipal(IEnumerable<Claim> claims)
    {
        // Passing a non-null authenticationType makes IsAuthenticated = true
        var identity = new ClaimsIdentity(claims, JwtBearerScheme);
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public void GetCurrentUserId_WhenUserIsAuthenticated_ReturnsUserId()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.User = AuthenticatedPrincipal(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "42")
        });
        var service = CreateServiceWithContext(context);

        // Act
        var result = service.GetCurrentUserId();

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void GetCurrentUserId_WhenHttpContextIsNull_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var service = CreateServiceWithContext(null);

        // Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() => service.GetCurrentUserId());
    }

    [Fact]
    public void GetCurrentUserId_WhenUserIsNotAuthenticated_ThrowsUnauthorizedAccessException()
    {
        // Arrange — ClaimsIdentity with no authenticationType means IsAuthenticated = false
        var context = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[] { new Claim(JwtRegisteredClaimNames.Sub, "1") });
        context.User = new ClaimsPrincipal(identity);
        var service = CreateServiceWithContext(context);

        // Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() => service.GetCurrentUserId());
    }

    [Fact]
    public void GetCurrentUserId_WhenSubClaimIsMissing_ThrowsUnauthorizedAccessException()
    {
        // Arrange — authenticated user but no "sub" claim
        var context = new DefaultHttpContext();
        context.User = AuthenticatedPrincipal(new[]
        {
            new Claim(ClaimTypes.Email, "user@example.com")
        });
        var service = CreateServiceWithContext(context);

        // Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() => service.GetCurrentUserId());
    }

    [Fact]
    public void GetCurrentUserId_WhenSubClaimIsNotInteger_ThrowsInvalidOperationException()
    {
        // Arrange — authenticated user with non-numeric "sub" claim
        var context = new DefaultHttpContext();
        context.User = AuthenticatedPrincipal(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "not-an-integer")
        });
        var service = CreateServiceWithContext(context);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.GetCurrentUserId());
    }
}
