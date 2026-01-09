using CncApp.Domain.Common;
using CncApp.Domain.Entities;
using Xunit;

namespace CncApp.Domain.Tests.Entities;

/// <summary>
/// Domain tests for User entity invariants.
/// Tests verify that invalid states cannot be created and that DomainException is thrown for violations.
/// These tests do NOT access the database or test application workflows.
/// </summary>
public class UserTests
{
    private const int ValidIdentityUserId = 1;
    private const string ValidUserName = "user@local.test";
    private const string ValidFirstName = "First";
    private const string ValidLastName = "Last";

    private const int MaxUserNameLength = 200;
    private const int MaxFirstNameLength = 200;
    private const int MaxLastNameLength = 200;

    #region Constructor Tests

    [Fact]
    public void Constructor_WhenIdentityUserIdIsZero_ThrowsDomainException()
    {
        // Arrange & Act
        var ex = Assert.Throws<DomainException>(() => new User
        {
            IdentityUserId = 0,
            UserName = ValidUserName
        });

        // Assert
        Assert.Contains("IdentityUserId", ex.Message);
    }

    [Fact]
    public void Constructor_WhenUserNameIsNull_ThrowsDomainException()
    {
        // Arrange
        string? userName = null;

        // Act
        var ex = Assert.Throws<DomainException>(() => new User
        {
            IdentityUserId = ValidIdentityUserId,
            UserName = userName!
        });

        // Assert
        Assert.Contains("UserName", ex.Message);
    }

    [Fact]
    public void Constructor_WhenUserNameIsWhitespace_ThrowsDomainException()
    {
        // Arrange
        var userName = "   ";

        // Act
        var ex = Assert.Throws<DomainException>(() => new User
        {
            IdentityUserId = ValidIdentityUserId,
            UserName = userName
        });

        // Assert
        Assert.Contains("UserName", ex.Message);
    }

    [Fact]
    public void Constructor_WhenUserNameExceedsMaxLength_ThrowsDomainException()
    {
        // Arrange
        var userName = new string('A', MaxUserNameLength + 1);

        // Act
        var ex = Assert.Throws<DomainException>(() => new User
        {
            IdentityUserId = ValidIdentityUserId,
            UserName = userName
        });

        // Assert
        Assert.Contains("UserName", ex.Message);
    }

    [Fact]
    public void Constructor_WhenValidParameters_CreatesUser()
    {
        // Arrange & Act
        var user = new User
        {
            IdentityUserId = ValidIdentityUserId,
            UserName = ValidUserName,
            FirstName = ValidFirstName,
            LastName = ValidLastName
        };

        // Assert
        Assert.NotNull(user);
        Assert.Equal(ValidIdentityUserId, user.IdentityUserId);
        Assert.Equal(ValidUserName, user.UserName);
        Assert.Equal(ValidFirstName, user.FirstName);
        Assert.Equal(ValidLastName, user.LastName);
        Assert.NotNull(user.Shifts);
        Assert.Empty(user.Shifts);
        Assert.Null(user.InactivatedDateTime);
    }

    #endregion

    #region Property Setter Tests

    [Fact]
    public void IdentityUserIdSetter_WhenValueIsZero_ThrowsDomainException()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        var ex = Assert.Throws<DomainException>(() => user.IdentityUserId = 0);

        // Assert
        Assert.Contains("IdentityUserId", ex.Message);
    }

    [Fact]
    public void UserNameSetter_WhenValueIsNull_ThrowsDomainException()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        var ex = Assert.Throws<DomainException>(() => user.UserName = null!);

        // Assert
        Assert.Contains("UserName", ex.Message);
    }

    [Fact]
    public void UserNameSetter_WhenValueIsWhitespace_ThrowsDomainException()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        var ex = Assert.Throws<DomainException>(() => user.UserName = "   ");

        // Assert
        Assert.Contains("UserName", ex.Message);
    }

    [Fact]
    public void UserNameSetter_WhenValueExceedsMaxLength_ThrowsDomainException()
    {
        // Arrange
        var user = CreateValidUser();
        var invalidUserName = new string('A', MaxUserNameLength + 1);

        // Act
        var ex = Assert.Throws<DomainException>(() => user.UserName = invalidUserName);

        // Assert
        Assert.Contains("UserName", ex.Message);
    }

    [Fact]
    public void FirstNameSetter_WhenValueExceedsMaxLength_ThrowsDomainException()
    {
        // Arrange
        var user = CreateValidUser();
        var invalidFirstName = new string('A', MaxFirstNameLength + 1);

        // Act
        var ex = Assert.Throws<DomainException>(() => user.FirstName = invalidFirstName);

        // Assert
        Assert.Contains("FirstName", ex.Message);
    }

    [Fact]
    public void LastNameSetter_WhenValueExceedsMaxLength_ThrowsDomainException()
    {
        // Arrange
        var user = CreateValidUser();
        var invalidLastName = new string('A', MaxLastNameLength + 1);

        // Act
        var ex = Assert.Throws<DomainException>(() => user.LastName = invalidLastName);

        // Assert
        Assert.Contains("LastName", ex.Message);
    }

    #endregion

    #region Method Tests

    [Fact]
    public void Inactivate_WhenUserIsActive_SetsInactivatedDateTime()
    {
        // Arrange
        var user = CreateValidUser();
        Assert.Null(user.InactivatedDateTime);

        // Act
        user.Inactivate();

        // Assert
        Assert.NotNull(user.InactivatedDateTime);
        Assert.True(user.InactivatedDateTime.Value <= DateTimeOffset.UtcNow);
        Assert.True(user.InactivatedDateTime.Value >= DateTimeOffset.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void Inactivate_WhenUserIsActive_SetsInactivatedByUserId()
    {
        // Arrange
        var user = CreateValidUser();
        var inactivatedByUserId = 42;

        // Act
        user.Inactivate(inactivatedByUserId);

        // Assert
        Assert.NotNull(user.InactivatedDateTime);
        Assert.Equal(inactivatedByUserId, user.InactivatedByUserId);
    }

    [Fact]
    public void Inactivate_WhenUserIsAlreadyInactivated_ThrowsDomainException()
    {
        // Arrange
        var user = CreateValidUser();
        user.Inactivate();

        // Act
        var ex = Assert.Throws<DomainException>(() => user.Inactivate());

        // Assert
        Assert.Contains("already inactivated", ex.Message);
    }

    #endregion

    private static User CreateValidUser() =>
        new()
        {
            IdentityUserId = ValidIdentityUserId,
            UserName = ValidUserName,
            FirstName = ValidFirstName,
            LastName = ValidLastName
        };
}


