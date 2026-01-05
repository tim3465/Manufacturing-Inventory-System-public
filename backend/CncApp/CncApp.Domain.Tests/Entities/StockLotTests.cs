using CncApp.Domain.Common;
using CncApp.Domain.Entities;
using CncApp.Domain.Enums;

namespace CncApp.Domain.Tests.Entities;

/// <summary>
/// Domain tests for StockLot entity invariants.
/// Tests verify that invalid states cannot be created and that DomainException is thrown for violations.
/// These tests do NOT access the database or test application workflows.
/// </summary>
public class StockLotTests
{
    private const string ValidLotNumber = "LOT-001";
    private const int ValidMaterialId = 1;
    private const int ValidAmountOfBars = 10;
    private const decimal ValidDiameter = 25.5m;
    private const decimal ValidBarLength = 1000.0m;
    private const StockLotConditionEnum ValidCondition = StockLotConditionEnum.AsReceived;
    private static readonly DateTime ValidCheckedInDateTime = new DateTime(2025, 1, 1, 10, 0, 0);
    private const int MaxLotNumberLength = 100;

    #region Constructor Tests

    [Fact]
    public void Constructor_WhenLotNumberIsNull_ThrowsDomainException()
    {
        // Arrange
        string? lotNumber = null;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new StockLot(
            lotNumber!,
            ValidMaterialId,
            ValidAmountOfBars,
            ValidDiameter,
            ValidBarLength,
            ValidCondition,
            ValidCheckedInDateTime));
        Assert.Contains("LotNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenLotNumberIsEmpty_ThrowsDomainException()
    {
        // Arrange
        var lotNumber = string.Empty;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new StockLot(
            lotNumber,
            ValidMaterialId,
            ValidAmountOfBars,
            ValidDiameter,
            ValidBarLength,
            ValidCondition,
            ValidCheckedInDateTime));
        Assert.Contains("LotNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenLotNumberIsWhitespace_ThrowsDomainException()
    {
        // Arrange
        var lotNumber = "   ";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new StockLot(
            lotNumber,
            ValidMaterialId,
            ValidAmountOfBars,
            ValidDiameter,
            ValidBarLength,
            ValidCondition,
            ValidCheckedInDateTime));
        Assert.Contains("LotNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenLotNumberExceedsMaxLength_ThrowsDomainException()
    {
        // Arrange
        var lotNumber = new string('A', MaxLotNumberLength + 1);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new StockLot(
            lotNumber,
            ValidMaterialId,
            ValidAmountOfBars,
            ValidDiameter,
            ValidBarLength,
            ValidCondition,
            ValidCheckedInDateTime));
        Assert.Contains("LotNumber cannot exceed", exception.Message);
        Assert.Contains($"{MaxLotNumberLength} characters", exception.Message);
    }

    [Fact]
    public void Constructor_WhenValidParameters_CreatesStockLot()
    {
        // Arrange & Act
        var stockLot = new StockLot(
            ValidLotNumber,
            ValidMaterialId,
            ValidAmountOfBars,
            ValidDiameter,
            ValidBarLength,
            ValidCondition,
            ValidCheckedInDateTime);

        // Assert
        Assert.NotNull(stockLot);
        Assert.Equal(ValidLotNumber, stockLot.LotNumber);
        Assert.Equal(ValidMaterialId, stockLot.MaterialId);
        Assert.Equal(ValidAmountOfBars, stockLot.AmountOfBars);
        Assert.Equal(ValidDiameter, stockLot.Diameter);
        Assert.Equal(ValidBarLength, stockLot.BarLength);
        Assert.Equal(ValidCondition, stockLot.Condition);
        Assert.Equal(ValidCheckedInDateTime, stockLot.CheckedInDateTime);
        Assert.NotNull(stockLot.StockLotAdjustments);
    }

    #endregion

    #region Property Setter Tests

    [Fact]
    public void LotNumber_WhenSetToNull_ThrowsDomainException()
    {
        // Arrange
        var stockLot = new StockLot(
            ValidLotNumber,
            ValidMaterialId,
            ValidAmountOfBars,
            ValidDiameter,
            ValidBarLength,
            ValidCondition,
            ValidCheckedInDateTime);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => stockLot.LotNumber = null!);
        Assert.Contains("LotNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void LotNumber_WhenSetToEmpty_ThrowsDomainException()
    {
        // Arrange
        var stockLot = new StockLot(
            ValidLotNumber,
            ValidMaterialId,
            ValidAmountOfBars,
            ValidDiameter,
            ValidBarLength,
            ValidCondition,
            ValidCheckedInDateTime);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => stockLot.LotNumber = string.Empty);
        Assert.Contains("LotNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void LotNumber_WhenSetToWhitespace_ThrowsDomainException()
    {
        // Arrange
        var stockLot = new StockLot(
            ValidLotNumber,
            ValidMaterialId,
            ValidAmountOfBars,
            ValidDiameter,
            ValidBarLength,
            ValidCondition,
            ValidCheckedInDateTime);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => stockLot.LotNumber = "   ");
        Assert.Contains("LotNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void LotNumber_WhenSetToValueExceedingMaxLength_ThrowsDomainException()
    {
        // Arrange
        var stockLot = new StockLot(
            ValidLotNumber,
            ValidMaterialId,
            ValidAmountOfBars,
            ValidDiameter,
            ValidBarLength,
            ValidCondition,
            ValidCheckedInDateTime);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => stockLot.LotNumber = new string('A', MaxLotNumberLength + 1));
        Assert.Contains("LotNumber cannot exceed", exception.Message);
        Assert.Contains($"{MaxLotNumberLength} characters", exception.Message);
    }

    [Fact]
    public void LotNumber_WhenSetToValidValue_UpdatesProperty()
    {
        // Arrange
        var stockLot = new StockLot(
            ValidLotNumber,
            ValidMaterialId,
            ValidAmountOfBars,
            ValidDiameter,
            ValidBarLength,
            ValidCondition,
            ValidCheckedInDateTime);
        var newLotNumber = "LOT-002";

        // Act
        stockLot.LotNumber = newLotNumber;

        // Assert
        Assert.Equal(newLotNumber, stockLot.LotNumber);
    }

    #endregion

    #region Method Tests

    [Fact]
    public void Inactivate_WhenStockLotIsActive_SetsInactivatedDateTime()
    {
        // Arrange
        var stockLot = new StockLot(
            ValidLotNumber,
            ValidMaterialId,
            ValidAmountOfBars,
            ValidDiameter,
            ValidBarLength,
            ValidCondition,
            ValidCheckedInDateTime);
        var userId = 1;

        // Act
        stockLot.Inactivate(userId);

        // Assert
        Assert.NotNull(stockLot.InactivatedDateTime);
        Assert.Equal(userId, stockLot.InactivatedByUserId);
    }

    [Fact]
    public void Inactivate_WhenStockLotIsAlreadyInactivated_ThrowsDomainException()
    {
        // Arrange
        var stockLot = new StockLot(
            ValidLotNumber,
            ValidMaterialId,
            ValidAmountOfBars,
            ValidDiameter,
            ValidBarLength,
            ValidCondition,
            ValidCheckedInDateTime);
        stockLot.Inactivate(1);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => stockLot.Inactivate(2));
        Assert.Contains("StockLot is already inactivated and cannot be inactivated again", exception.Message);
    }

    [Fact]
    public void Inactivate_WhenUserIdIsNull_SetsInactivatedDateTime()
    {
        // Arrange
        var stockLot = new StockLot(
            ValidLotNumber,
            ValidMaterialId,
            ValidAmountOfBars,
            ValidDiameter,
            ValidBarLength,
            ValidCondition,
            ValidCheckedInDateTime);

        // Act
        stockLot.Inactivate(null);

        // Assert
        Assert.NotNull(stockLot.InactivatedDateTime);
        Assert.Null(stockLot.InactivatedByUserId);
    }

    #endregion
}

