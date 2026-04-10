using CncApp.Domain.Common;
using CncApp.Domain.Entities;
using CncApp.Domain.Enums;

namespace CncApp.Domain.Tests.Entities;

/// <summary>
/// Domain tests for ShiftIssueLog entity invariants.
/// Tests verify that invalid states cannot be created and that DomainException is thrown for violations.
/// These tests do NOT access the database or test application workflows.
/// </summary>
public class ShiftIssueLogTests
{
    private const int ValidShiftId = 1;
    private const IssueTypeEnum ValidIssueType = IssueTypeEnum.Setup;
    private const int ValidScrapQuantity = 5;
    private const string ValidDescription = "Test description";
    private const int MaxDescriptionLength = 2000;

    #region Constructor Tests

    [Fact]
    public void Constructor_WhenShiftIdIsZero_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new ShiftIssueLog(0, ValidIssueType, ValidScrapQuantity, ValidDescription));
        Assert.Contains("ShiftId must be greater than 0", exception.Message);
    }

    [Fact]
    public void Constructor_WhenShiftIdIsNegative_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new ShiftIssueLog(-1, ValidIssueType, ValidScrapQuantity, ValidDescription));
        Assert.Contains("ShiftId must be greater than 0", exception.Message);
    }

    [Fact]
    public void Constructor_WhenScrapQuantityIsNegative_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new ShiftIssueLog(ValidShiftId, ValidIssueType, -1, ValidDescription));
        Assert.Contains("ScrapQuantity must be non-negative", exception.Message);
    }

    [Fact]
    public void Constructor_WhenDescriptionIsNull_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, null!));
        Assert.Contains("Description cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenDescriptionIsEmpty_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, ""));
        Assert.Contains("Description cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenDescriptionIsWhitespace_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, "   "));
        Assert.Contains("Description cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenDescriptionExceedsMaxLength_ThrowsDomainException()
    {
        var description = new string('A', MaxDescriptionLength + 1);

        var exception = Assert.Throws<DomainException>(() =>
            new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, description));
        Assert.Contains("Description cannot exceed", exception.Message);
        Assert.Contains($"{MaxDescriptionLength} characters", exception.Message);
    }

    [Fact]
    public void Constructor_WhenDescriptionIsMaxLength_CreatesShiftIssueLog()
    {
        var description = new string('A', MaxDescriptionLength);

        var log = new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, description);

        Assert.NotNull(log);
        Assert.Equal(description, log.Description);
    }

    [Fact]
    public void Constructor_WhenBothScrapAndDowntimeAreZeroOrNull_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new ShiftIssueLog(ValidShiftId, ValidIssueType, 0, ValidDescription, null));
        Assert.Contains("At least one of ScrapQuantity or Downtime must have a non-zero value", exception.Message);
    }

    [Fact]
    public void Constructor_WhenScrapIsZeroAndDowntimeIsZero_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new ShiftIssueLog(ValidShiftId, ValidIssueType, 0, ValidDescription, TimeSpan.Zero));
        Assert.Contains("At least one of ScrapQuantity or Downtime must have a non-zero value", exception.Message);
    }

    [Fact]
    public void Constructor_WhenValidWithScrapOnly_CreatesShiftIssueLog()
    {
        var log = new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, ValidDescription);

        Assert.NotNull(log);
        Assert.Equal(ValidShiftId, log.ShiftId);
        Assert.Equal(ValidIssueType, log.IssueType);
        Assert.Equal(ValidScrapQuantity, log.ScrapQuantity);
        Assert.Equal(ValidDescription, log.Description);
        Assert.Null(log.Downtime);
        Assert.Null(log.InactivatedDateTime);
    }

    [Fact]
    public void Constructor_WhenValidWithDowntimeOnly_CreatesShiftIssueLog()
    {
        var downtime = TimeSpan.FromMinutes(30);

        var log = new ShiftIssueLog(ValidShiftId, ValidIssueType, 0, ValidDescription, downtime);

        Assert.NotNull(log);
        Assert.Equal(0, log.ScrapQuantity);
        Assert.Equal(downtime, log.Downtime);
    }

    [Fact]
    public void Constructor_WhenValidWithBothScrapAndDowntime_CreatesShiftIssueLog()
    {
        var downtime = TimeSpan.FromMinutes(15);

        var log = new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, ValidDescription, downtime);

        Assert.NotNull(log);
        Assert.Equal(ValidScrapQuantity, log.ScrapQuantity);
        Assert.Equal(downtime, log.Downtime);
    }

    #endregion

    #region Property Setter Tests

    [Fact]
    public void ShiftIdSetter_WhenValueIsZero_ThrowsDomainException()
    {
        var log = new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, ValidDescription);

        var exception = Assert.Throws<DomainException>(() => log.ShiftId = 0);
        Assert.Contains("ShiftId must be greater than 0", exception.Message);
    }

    [Fact]
    public void ShiftIdSetter_WhenValueIsNegative_ThrowsDomainException()
    {
        var log = new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, ValidDescription);

        var exception = Assert.Throws<DomainException>(() => log.ShiftId = -1);
        Assert.Contains("ShiftId must be greater than 0", exception.Message);
    }

    [Fact]
    public void ShiftIdSetter_WhenValueIsValid_UpdatesProperty()
    {
        var log = new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, ValidDescription);

        log.ShiftId = 2;

        Assert.Equal(2, log.ShiftId);
    }

    [Fact]
    public void ScrapQuantitySetter_WhenValueIsNegative_ThrowsDomainException()
    {
        var log = new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, ValidDescription);

        var exception = Assert.Throws<DomainException>(() => log.ScrapQuantity = -1);
        Assert.Contains("ScrapQuantity must be non-negative", exception.Message);
    }

    [Fact]
    public void ScrapQuantitySetter_WhenValueIsZero_UpdatesProperty()
    {
        var log = new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, ValidDescription);

        log.ScrapQuantity = 0;

        Assert.Equal(0, log.ScrapQuantity);
    }

    [Fact]
    public void ScrapQuantitySetter_WhenValueIsValid_UpdatesProperty()
    {
        var log = new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, ValidDescription);

        log.ScrapQuantity = 10;

        Assert.Equal(10, log.ScrapQuantity);
    }

    [Fact]
    public void DescriptionSetter_WhenValueIsNull_ThrowsDomainException()
    {
        var log = new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, ValidDescription);

        var exception = Assert.Throws<DomainException>(() => log.Description = null!);
        Assert.Contains("Description cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void DescriptionSetter_WhenValueIsEmpty_ThrowsDomainException()
    {
        var log = new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, ValidDescription);

        var exception = Assert.Throws<DomainException>(() => log.Description = "");
        Assert.Contains("Description cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void DescriptionSetter_WhenValueIsWhitespace_ThrowsDomainException()
    {
        var log = new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, ValidDescription);

        var exception = Assert.Throws<DomainException>(() => log.Description = "   ");
        Assert.Contains("Description cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void DescriptionSetter_WhenValueExceedsMaxLength_ThrowsDomainException()
    {
        var log = new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, ValidDescription);
        var invalidDescription = new string('A', MaxDescriptionLength + 1);

        var exception = Assert.Throws<DomainException>(() => log.Description = invalidDescription);
        Assert.Contains("Description cannot exceed", exception.Message);
        Assert.Contains($"{MaxDescriptionLength} characters", exception.Message);
    }

    [Fact]
    public void DescriptionSetter_WhenValueIsMaxLength_UpdatesProperty()
    {
        var log = new ShiftIssueLog(ValidShiftId, ValidIssueType, ValidScrapQuantity, ValidDescription);
        var validDescription = new string('A', MaxDescriptionLength);

        log.Description = validDescription;

        Assert.Equal(validDescription, log.Description);
    }

    #endregion
}
