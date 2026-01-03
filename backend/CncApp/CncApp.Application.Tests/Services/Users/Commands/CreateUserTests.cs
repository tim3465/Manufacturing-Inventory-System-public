using CncApp.Application.Dtos.Users;
using CncApp.Application.Interfaces;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Users;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Users.Commands;

public class CreateUserTests
{
    private readonly Mock<IIdentityProvisioningService> _mockIdentityService;
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly UserService _userService;

    public CreateUserTests()
    {
        _mockIdentityService = new Mock<IIdentityProvisioningService>();
        _mockRepository = new Mock<IUserRepository>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _userService = new UserService(
            _mockIdentityService.Object,
            _mockRepository.Object,
            _mockCurrentUserService.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenValidRequest_Succeeds()
    {
        // Arrange
        var identityUserId = 10;
        var domainUserId = 20;
        var cancellationToken = CancellationToken.None;

        var requestDto = new CreateUserRequestDto
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            TemporaryPassword = "TempPass123!",
            Roles = new List<string> { "Admin", "User" }
        };

        _mockIdentityService
            .Setup(s => s.CreateIdentityUserAsync(requestDto.Email, requestDto.Email, requestDto.TemporaryPassword, cancellationToken))
            .ReturnsAsync(identityUserId);

        _mockIdentityService
            .Setup(s => s.AssignRolesAsync(identityUserId, requestDto.Roles, cancellationToken))
            .Returns(Task.CompletedTask);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<User>(), cancellationToken))
            .Callback<User, CancellationToken>((user, ct) =>
            {
                // Simulate EF setting the Id after SaveChangesAsync
                user.Id = domainUserId;
            })
            .Returns(Task.CompletedTask);

        _mockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.CreateAsync(requestDto, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(identityUserId, result.IdentityUserId);
        Assert.Equal(domainUserId, result.DomainUserId);
        Assert.Equal(requestDto.Email, result.UserName);

        _mockIdentityService.Verify(
            s => s.CreateIdentityUserAsync(requestDto.Email, requestDto.Email, requestDto.TemporaryPassword, cancellationToken),
            Times.Once);
        _mockIdentityService.Verify(
            s => s.AssignRolesAsync(identityUserId, requestDto.Roles, cancellationToken),
            Times.Once);
        _mockRepository.Verify(
            r => r.AddAsync(It.Is<User>(u => 
                u.IdentityUserId == identityUserId &&
                u.UserName == requestDto.Email &&
                u.FirstName == requestDto.FirstName &&
                u.LastName == requestDto.LastName),
            cancellationToken),
            Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenInvalidOrDuplicateRequest_Fails()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var requestDto = new CreateUserRequestDto
        {
            Email = "duplicate@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            TemporaryPassword = "TempPass123!",
            Roles = new List<string>()
        };

        _mockIdentityService
            .Setup(s => s.CreateIdentityUserAsync(requestDto.Email, requestDto.Email, requestDto.TemporaryPassword, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Failed to create Identity user: Duplicate email address"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _userService.CreateAsync(requestDto, cancellationToken));

        _mockIdentityService.Verify(
            s => s.CreateIdentityUserAsync(requestDto.Email, requestDto.Email, requestDto.TemporaryPassword, cancellationToken),
            Times.Once);
        _mockIdentityService.Verify(
            s => s.AssignRolesAsync(It.IsAny<int>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
            Times.Never());
        _mockRepository.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never());
        _mockRepository.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never());
    }
}

