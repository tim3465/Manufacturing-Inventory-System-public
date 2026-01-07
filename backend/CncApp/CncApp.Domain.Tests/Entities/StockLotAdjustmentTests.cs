using CncApp.Domain.Common;
using CncApp.Domain.Entities;
using CncApp.Domain.Enums;

namespace CncApp.Domain.Tests.Entities;

/// <summary>
/// Domain tests for StockLotAdjustment entity invariants.
/// Tests verify that invalid states cannot be created and that DomainException is thrown for violations.
/// These tests do NOT access the database or test application workflows.
/// </summary>
public class StockLotAdjustmentTests
{
    private const int ValidStockLotId = 1;
    private const int ValidDeltaBars = 10;
    private const StockLotAdjustmentReasonEnum ValidReason = StockLotAdjustmentReasonEnum.Received;
    private const int MaxNotesLength = 2000;

    #region Constructor Tests

    [Fact]
    public void Constructor_WhenStockLotIdIsZero_ThrowsDomainException()
    {
        // Arrange
        var stockLotId = 0;
        var deltaBars = ValidDeltaBars;
        var reason = ValidReason;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new StockLotAdjustment(stockLotId, deltaBars, reason));
        Assert.Contains("StockLotId must be greater than 0", exception.Message);
    }

    [Fact]
    public void Constructor_WhenStockLotIdIsNegative_ThrowsDomainException()
    {
        // Arrange
        var stockLotId = -1;
        var deltaBars = ValidDeltaBars;
        var reason = ValidReason;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new StockLotAdjustment(stockLotId, deltaBars, reason));
        Assert.Contains("StockLotId must be greater than 0", exception.Message);
    }

    [Fact]
    public void Constructor_WhenNotesExceedsMaxLength_ThrowsDomainException()
    {
        // Arrange
        var stockLotId = ValidStockLotId;
        var deltaBars = ValidDeltaBars;
        var reason = ValidReason;
        var notes = new string('A', MaxNotesLength + 1);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new StockLotAdjustment(stockLotId, deltaBars, reason, null, notes));
        Assert.Contains("Notes cannot exceed", exception.Message);
        Assert.Contains($"{MaxNotesLength} characters", exception.Message);
    }

    [Fact]
    public void Constructor_WhenValidParameters_CreatesStockLotAdjustment()
    {
        // Arrange
        var stockLotId = ValidStockLotId;
        var deltaBars = ValidDeltaBars;
        var reason = ValidReason;
        var jobId = 5;
        var notes = "Test notes";

        // Act
        var adjustment = new StockLotAdjustment(stockLotId, deltaBars, reason, jobId, notes);

        // Assert
        Assert.NotNull(adjustment);
        Assert.Equal(stockLotId, adjustment.StockLotId);
        Assert.Equal(deltaBars, adjustment.DeltaBars);
        Assert.Equal(reason, adjustment.Reason);
        Assert.Equal(jobId, adjustment.JobId);
        Assert.Equal(notes, adjustment.Notes);
        Assert.Null(adjustment.InactivatedDateTime);
    }

    [Fact]
    public void Constructor_WhenValidParameters_WithNullJobId_CreatesStockLotAdjustment()
    {
        // Arrange
        var stockLotId = ValidStockLotId;
        var deltaBars = ValidDeltaBars;
        var reason = ValidReason;

        // Act
        var adjustment = new StockLotAdjustment(stockLotId, deltaBars, reason);

        // Assert
        Assert.NotNull(adjustment);
        Assert.Equal(stockLotId, adjustment.StockLotId);
        Assert.Equal(deltaBars, adjustment.DeltaBars);
        Assert.Equal(reason, adjustment.Reason);
        Assert.Null(adjustment.JobId);
        Assert.Null(adjustment.Notes);
    }

    [Fact]
    public void Constructor_WhenNotesIsMaxLength_CreatesStockLotAdjustment()
    {
        // Arrange
        var stockLotId = ValidStockLotId;
        var deltaBars = ValidDeltaBars;
        var reason = ValidReason;
        var notes = new string('A', MaxNotesLength);

        // Act
        var adjustment = new StockLotAdjustment(stockLotId, deltaBars, reason, null, notes);

        // Assert
        Assert.NotNull(adjustment);
        Assert.Equal(notes, adjustment.Notes);
    }

    #endregion

    #region Property Setter Tests

    [Fact]
    public void StockLotIdSetter_WhenValueIsZero_ThrowsDomainException()
    {
        // Arrange
        var adjustment = new StockLotAdjustment(ValidStockLotId, ValidDeltaBars, ValidReason);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => adjustment.StockLotId = 0);
        Assert.Contains("StockLotId must be greater than 0", exception.Message);
    }

    [Fact]
    public void StockLotIdSetter_WhenValueIsNegative_ThrowsDomainException()
    {
        // Arrange
        var adjustment = new StockLotAdjustment(ValidStockLotId, ValidDeltaBars, ValidReason);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => adjustment.StockLotId = -1);
        Assert.Contains("StockLotId must be greater than 0", exception.Message);
    }

    [Fact]
    public void StockLotIdSetter_WhenValueIsValid_UpdatesProperty()
    {
        // Arrange
        var adjustment = new StockLotAdjustment(ValidStockLotId, ValidDeltaBars, ValidReason);
        var newStockLotId = 2;

        // Act
        adjustment.StockLotId = newStockLotId;

        // Assert
        Assert.Equal(newStockLotId, adjustment.StockLotId);
    }

    [Fact]
    public void NotesSetter_WhenValueExceedsMaxLength_ThrowsDomainException()
    {
        // Arrange
        var adjustment = new StockLotAdjustment(ValidStockLotId, ValidDeltaBars, ValidReason);
        var invalidNotes = new string('A', MaxNotesLength + 1);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => adjustment.Notes = invalidNotes);
        Assert.Contains("Notes cannot exceed", exception.Message);
    }

    [Fact]
    public void NotesSetter_WhenValueIsMaxLength_UpdatesProperty()
    {
        // Arrange
        var adjustment = new StockLotAdjustment(ValidStockLotId, ValidDeltaBars, ValidReason);
        var validNotes = new string('A', MaxNotesLength);

        // Act
        adjustment.Notes = validNotes;

        // Assert
        Assert.Equal(validNotes, adjustment.Notes);
    }

    [Fact]
    public void NotesSetter_WhenValueIsNull_UpdatesProperty()
    {
        // Arrange
        var adjustment = new StockLotAdjustment(ValidStockLotId, ValidDeltaBars, ValidReason, null, "Initial notes");

        // Act
        adjustment.Notes = null;

        // Assert
        Assert.Null(adjustment.Notes);
    }

    #endregion

    #region Method Tests

    [Fact]
    public void Inactivate_WhenAdjustmentIsActive_SetsInactivatedDateTime()
    {
        // Arrange
        var adjustment = new StockLotAdjustment(ValidStockLotId, ValidDeltaBars, ValidReason);
        Assert.Null(adjustment.InactivatedDateTime);

        // Act
        adjustment.Inactivate();

        // Assert
        Assert.NotNull(adjustment.InactivatedDateTime);
        Assert.True(adjustment.InactivatedDateTime.Value <= DateTimeOffset.UtcNow);
        Assert.True(adjustment.InactivatedDateTime.Value >= DateTimeOffset.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void Inactivate_WhenAdjustmentIsActive_SetsInactivatedByUserId()
    {
        // Arrange
        var adjustment = new StockLotAdjustment(ValidStockLotId, ValidDeltaBars, ValidReason);
        var userId = 42;

        // Act
        adjustment.Inactivate(userId);

        // Assert
        Assert.NotNull(adjustment.InactivatedDateTime);
        Assert.Equal(userId, adjustment.InactivatedByUserId);
    }

    [Fact]
    public void Inactivate_WhenAdjustmentIsActive_WithNullUserId_SetsInactivatedDateTime()
    {
        // Arrange
        var adjustment = new StockLotAdjustment(ValidStockLotId, ValidDeltaBars, ValidReason);

        // Act
        adjustment.Inactivate(null);

        // Assert
        Assert.NotNull(adjustment.InactivatedDateTime);
        Assert.Null(adjustment.InactivatedByUserId);
    }

    [Fact]
    public void Inactivate_WhenAdjustmentIsAlreadyInactivated_ThrowsDomainException()
    {
        // Arrange
        var adjustment = new StockLotAdjustment(ValidStockLotId, ValidDeltaBars, ValidReason);
        adjustment.Inactivate();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => adjustment.Inactivate());
        Assert.Contains("already inactivated", exception.Message);
        Assert.Contains("cannot be inactivated again", exception.Message);
    }

    [Fact]
    public void Inactivate_WhenAdjustmentIsAlreadyInactivated_WithUserId_ThrowsDomainException()
    {
        // Arrange
        var adjustment = new StockLotAdjustment(ValidStockLotId, ValidDeltaBars, ValidReason);
        adjustment.Inactivate(1);
        var originalInactivatedDateTime = adjustment.InactivatedDateTime;
        var originalInactivatedByUserId = adjustment.InactivatedByUserId;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => adjustment.Inactivate(2));
        Assert.Contains("already inactivated", exception.Message);

        // Assert that state was not changed
        Assert.Equal(originalInactivatedDateTime, adjustment.InactivatedDateTime);
        Assert.Equal(originalInactivatedByUserId, adjustment.InactivatedByUserId);
    }

    #endregion
}

