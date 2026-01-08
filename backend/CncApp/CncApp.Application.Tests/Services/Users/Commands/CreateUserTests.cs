using CncApp.Application.Dtos.Users;
using CncApp.Application.Services.Users;
using CncApp.Domain.Entities;
using Moq;

namespace CncApp.Application.Tests.Services.Users;

public partial class UserTests
{
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

        MockIdentityProvisioningService
            .Setup(s => s.CreateIdentityUserAsync(requestDto.Email, requestDto.Email, requestDto.TemporaryPassword, cancellationToken))
            .ReturnsAsync(identityUserId);

        MockIdentityProvisioningService
            .Setup(s => s.AssignRolesAsync(identityUserId, requestDto.Roles, cancellationToken))
            .Returns(Task.CompletedTask);

        MockRepository
            .Setup(r => r.AddAsync(It.IsAny<User>(), cancellationToken))
            .Callback<User, CancellationToken>((user, ct) =>
            {
                // Simulate EF setting the Id after SaveChangesAsync
                user.Id = domainUserId;
            })
            .Returns(Task.CompletedTask);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await UserService.CreateAsync(requestDto, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(identityUserId, result.IdentityUserId);
        Assert.Equal(domainUserId, result.DomainUserId);
        Assert.Equal(requestDto.Email, result.UserName);

        MockIdentityProvisioningService.Verify(
            s => s.CreateIdentityUserAsync(requestDto.Email, requestDto.Email, requestDto.TemporaryPassword, cancellationToken),
            Times.Once);
        MockIdentityProvisioningService.Verify(
            s => s.AssignRolesAsync(identityUserId, requestDto.Roles, cancellationToken),
            Times.Once);
        MockRepository.Verify(
            r => r.AddAsync(It.Is<User>(u => 
                u.IdentityUserId == identityUserId &&
                u.UserName == requestDto.Email &&
                u.FirstName == requestDto.FirstName &&
                u.LastName == requestDto.LastName),
            cancellationToken),
            Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
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

        MockIdentityProvisioningService
            .Setup(s => s.CreateIdentityUserAsync(requestDto.Email, requestDto.Email, requestDto.TemporaryPassword, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Failed to create Identity user: Duplicate email address"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            UserService.CreateAsync(requestDto, cancellationToken));

        MockIdentityProvisioningService.Verify(
            s => s.CreateIdentityUserAsync(requestDto.Email, requestDto.Email, requestDto.TemporaryPassword, cancellationToken),
            Times.Once);
        MockIdentityProvisioningService.Verify(
            s => s.AssignRolesAsync(It.IsAny<int>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
            Times.Never());
        MockRepository.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never());
        MockRepository.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never());
    }
}

