using CncApp.Domain.Common;
using CncApp.Domain.Entities;

namespace CncApp.Domain.Tests.Entities;

/// <summary>
/// Domain tests for Part entity invariants.
/// Tests verify that invalid states cannot be created and that DomainException is thrown for violations.
/// These tests do NOT access the database or test application workflows.
/// </summary>
public class PartTests
{
    private static readonly TimeSpan ValidApproxPartCycleTime = TimeSpan.FromMinutes(5);
    private const int ValidCheckPerPart = 10;
    private const string ValidPartName = "Test Part";
    private const string ValidPartNumber = "TP-001";

    #region Constructor Tests

    [Fact]
    public void Constructor_WhenApproxPartCycleTimeIsNegative_ThrowsDomainException()
    {
        // Arrange
        var approxPartCycleTime = TimeSpan.FromMinutes(-1);
        var checkPerPart = ValidCheckPerPart;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Part("Test Part", "TP-001", approxPartCycleTime, checkPerPart));
        Assert.Contains("ApproxPartCycleTime must be non-negative", exception.Message);
    }

    [Fact]
    public void Constructor_WhenCheckPerPartIsNegative_ThrowsDomainException()
    {
        // Arrange
        var approxPartCycleTime = ValidApproxPartCycleTime;
        var checkPerPart = -1;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Part("Test Part", "TP-001", approxPartCycleTime, checkPerPart));
        Assert.Contains("CheckPerPart must be non-negative", exception.Message);
    }

    [Fact]
    public void Constructor_WhenValidParameters_CreatesPart()
    {
        // Arrange
        var approxPartCycleTime = ValidApproxPartCycleTime;
        var checkPerPart = ValidCheckPerPart;

        // Act
        var part = new Part("Test Part", "TP-001", approxPartCycleTime, checkPerPart);

        // Assert
        Assert.NotNull(part);
        Assert.Equal(approxPartCycleTime, part.ApproxPartCycleTime);
        Assert.Equal(checkPerPart, part.CheckPerPart);
        Assert.NotNull(part.Orders);
        Assert.Empty(part.Orders);
        Assert.Null(part.InactivatedDateTime);
    }

    [Fact]
    public void Constructor_WhenApproxPartCycleTimeIsZero_CreatesPart()
    {
        // Arrange
        var approxPartCycleTime = TimeSpan.Zero;
        var checkPerPart = ValidCheckPerPart;

        // Act
        var part = new Part("Test Part", "TP-001", approxPartCycleTime, checkPerPart);

        // Assert
        Assert.NotNull(part);
        Assert.Equal(TimeSpan.Zero, part.ApproxPartCycleTime);
    }

    [Fact]
    public void Constructor_WhenCheckPerPartIsZero_CreatesPart()
    {
        // Arrange
        var approxPartCycleTime = ValidApproxPartCycleTime;
        var checkPerPart = 0;

        // Act
        var part = new Part("Test Part", "TP-001", approxPartCycleTime, checkPerPart);

        // Assert
        Assert.NotNull(part);
        Assert.Equal(0, part.CheckPerPart);
    }

    #endregion

    #region Property Setter Tests

    [Fact]
    public void ApproxPartCycleTimeSetter_WhenValueIsNegative_ThrowsDomainException()
    {
        // Arrange
        var part = new Part("Test Part", "TP-001", ValidApproxPartCycleTime, ValidCheckPerPart);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => part.ApproxPartCycleTime = TimeSpan.FromMinutes(-1));
        Assert.Contains("ApproxPartCycleTime must be non-negative", exception.Message);
    }

    [Fact]
    public void ApproxPartCycleTimeSetter_WhenValueIsValid_UpdatesProperty()
    {
        // Arrange
        var part = new Part("Test Part", "TP-001", ValidApproxPartCycleTime, ValidCheckPerPart);
        var newCycleTime = TimeSpan.FromMinutes(10);

        // Act
        part.ApproxPartCycleTime = newCycleTime;

        // Assert
        Assert.Equal(newCycleTime, part.ApproxPartCycleTime);
    }

    [Fact]
    public void ApproxPartCycleTimeSetter_WhenValueIsZero_UpdatesProperty()
    {
        // Arrange
        var part = new Part("Test Part", "TP-001", ValidApproxPartCycleTime, ValidCheckPerPart);

        // Act
        part.ApproxPartCycleTime = TimeSpan.Zero;

        // Assert
        Assert.Equal(TimeSpan.Zero, part.ApproxPartCycleTime);
    }

    [Fact]
    public void CheckPerPartSetter_WhenValueIsNegative_ThrowsDomainException()
    {
        // Arrange
        var part = new Part("Test Part", "TP-001", ValidApproxPartCycleTime, ValidCheckPerPart);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => part.CheckPerPart = -1);
        Assert.Contains("CheckPerPart must be non-negative", exception.Message);
    }

    [Fact]
    public void CheckPerPartSetter_WhenValueIsValid_UpdatesProperty()
    {
        // Arrange
        var part = new Part("Test Part", "TP-001", ValidApproxPartCycleTime, ValidCheckPerPart);
        var newCheckPerPart = 20;

        // Act
        part.CheckPerPart = newCheckPerPart;

        // Assert
        Assert.Equal(newCheckPerPart, part.CheckPerPart);
    }

    [Fact]
    public void CheckPerPartSetter_WhenValueIsZero_UpdatesProperty()
    {
        // Arrange
        var part = new Part("Test Part", "TP-001", ValidApproxPartCycleTime, ValidCheckPerPart);

        // Act
        part.CheckPerPart = 0;

        // Assert
        Assert.Equal(0, part.CheckPerPart);
    }

    #endregion

    #region PartName and PartNumber Tests

    [Fact]
    public void Constructor_WhenPartNameIsNull_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Part(null!, ValidPartNumber, ValidApproxPartCycleTime, ValidCheckPerPart));
        Assert.Contains("PartName", exception.Message);
    }

    [Fact]
    public void Constructor_WhenPartNameIsWhiteSpace_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Part("   ", ValidPartNumber, ValidApproxPartCycleTime, ValidCheckPerPart));
        Assert.Contains("PartName", exception.Message);
    }

    [Fact]
    public void Constructor_WhenPartNameExceedsMaxLength_ThrowsDomainException()
    {
        var longName = new string('A', 101);
        var exception = Assert.Throws<DomainException>(() =>
            new Part(longName, ValidPartNumber, ValidApproxPartCycleTime, ValidCheckPerPart));
        Assert.Contains("PartName", exception.Message);
    }

    [Fact]
    public void Constructor_WhenPartNumberIsNull_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Part(ValidPartName, null!, ValidApproxPartCycleTime, ValidCheckPerPart));
        Assert.Contains("PartNumber", exception.Message);
    }

    [Fact]
    public void Constructor_WhenPartNumberIsWhiteSpace_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Part(ValidPartName, "   ", ValidApproxPartCycleTime, ValidCheckPerPart));
        Assert.Contains("PartNumber", exception.Message);
    }

    [Fact]
    public void Constructor_WhenPartNumberExceedsMaxLength_ThrowsDomainException()
    {
        var longNumber = new string('X', 51);
        var exception = Assert.Throws<DomainException>(() =>
            new Part(ValidPartName, longNumber, ValidApproxPartCycleTime, ValidCheckPerPart));
        Assert.Contains("PartNumber", exception.Message);
    }

    [Fact]
    public void PartNameSetter_WhenValueIsNull_ThrowsDomainException()
    {
        var part = new Part(ValidPartName, ValidPartNumber, ValidApproxPartCycleTime, ValidCheckPerPart);
        var exception = Assert.Throws<DomainException>(() => part.PartName = null!);
        Assert.Contains("PartName", exception.Message);
    }

    [Fact]
    public void PartNameSetter_WhenValueIsValid_UpdatesProperty()
    {
        var part = new Part(ValidPartName, ValidPartNumber, ValidApproxPartCycleTime, ValidCheckPerPart);
        part.PartName = "Updated Part";
        Assert.Equal("Updated Part", part.PartName);
    }

    [Fact]
    public void PartNumberSetter_WhenValueIsNull_ThrowsDomainException()
    {
        var part = new Part(ValidPartName, ValidPartNumber, ValidApproxPartCycleTime, ValidCheckPerPart);
        var exception = Assert.Throws<DomainException>(() => part.PartNumber = null!);
        Assert.Contains("PartNumber", exception.Message);
    }

    [Fact]
    public void PartNumberSetter_WhenValueIsValid_UpdatesProperty()
    {
        var part = new Part(ValidPartName, ValidPartNumber, ValidApproxPartCycleTime, ValidCheckPerPart);
        part.PartNumber = "TP-999";
        Assert.Equal("TP-999", part.PartNumber);
    }

    #endregion

    #region Inactivate Method Tests

    [Fact]
    public void Inactivate_WhenPartIsActive_SetsInactivatedDateTime()
    {
        // Arrange
        var part = new Part("Test Part", "TP-001", ValidApproxPartCycleTime, ValidCheckPerPart);
        Assert.Null(part.InactivatedDateTime);

        // Act
        part.Inactivate();

        // Assert
        Assert.NotNull(part.InactivatedDateTime);
        Assert.True(part.InactivatedDateTime.Value <= DateTimeOffset.UtcNow);
        Assert.True(part.InactivatedDateTime.Value >= DateTimeOffset.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void Inactivate_WhenPartIsActive_SetsInactivatedByUserId()
    {
        // Arrange
        var part = new Part("Test Part", "TP-001", ValidApproxPartCycleTime, ValidCheckPerPart);
        var userId = 42;

        // Act
        part.Inactivate(userId);

        // Assert
        Assert.NotNull(part.InactivatedDateTime);
        Assert.Equal(userId, part.InactivatedByUserId);
    }

    [Fact]
    public void Inactivate_WhenPartIsActive_WithNullUserId_SetsInactivatedDateTime()
    {
        // Arrange
        var part = new Part("Test Part", "TP-001", ValidApproxPartCycleTime, ValidCheckPerPart);

        // Act
        part.Inactivate(null);

        // Assert
        Assert.NotNull(part.InactivatedDateTime);
        Assert.Null(part.InactivatedByUserId);
    }

    [Fact]
    public void Inactivate_WhenPartIsAlreadyInactivated_ThrowsDomainException()
    {
        // Arrange
        var part = new Part("Test Part", "TP-001", ValidApproxPartCycleTime, ValidCheckPerPart);
        part.Inactivate();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => part.Inactivate());
        Assert.Contains("already inactivated", exception.Message);
        Assert.Contains("cannot be inactivated again", exception.Message);
    }

    [Fact]
    public void Inactivate_WhenPartIsAlreadyInactivated_WithUserId_ThrowsDomainException()
    {
        // Arrange
        var part = new Part("Test Part", "TP-001", ValidApproxPartCycleTime, ValidCheckPerPart);
        part.Inactivate(1);
        var originalInactivatedDateTime = part.InactivatedDateTime;
        var originalInactivatedByUserId = part.InactivatedByUserId;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => part.Inactivate(2));
        Assert.Contains("already inactivated", exception.Message);

        // Assert that state was not changed
        Assert.Equal(originalInactivatedDateTime, part.InactivatedDateTime);
        Assert.Equal(originalInactivatedByUserId, part.InactivatedByUserId);
    }

    #endregion
}

