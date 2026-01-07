using CncApp.Domain.Common;
using CncApp.Domain.Entities;

namespace CncApp.Domain.Tests.Entities;

/// <summary>
/// Domain tests for Material entity invariants.
/// Tests verify that invalid states cannot be created and that DomainException is thrown for violations.
/// These tests do NOT access the database or test application workflows.
/// </summary>
public class MaterialTests
{
    private const string ValidHeatNumber = "HN123456";
    private const string ValidMaterialName = "Steel-A1";
    private const int MaxLength = 100;

    #region Constructor Tests

    [Fact]
    public void Constructor_WhenHeatNumberIsNull_ThrowsDomainException()
    {
        // Arrange
        string? heatNumber = null;
        var materialName = ValidMaterialName;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Material(heatNumber!, materialName));
        Assert.Contains("HeatNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenHeatNumberIsEmpty_ThrowsDomainException()
    {
        // Arrange
        var heatNumber = string.Empty;
        var materialName = ValidMaterialName;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Material(heatNumber, materialName));
        Assert.Contains("HeatNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenHeatNumberIsWhitespace_ThrowsDomainException()
    {
        // Arrange
        var heatNumber = "   ";
        var materialName = ValidMaterialName;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Material(heatNumber, materialName));
        Assert.Contains("HeatNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenHeatNumberExceedsMaxLength_ThrowsDomainException()
    {
        // Arrange
        var heatNumber = new string('A', MaxLength + 1);
        var materialName = ValidMaterialName;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Material(heatNumber, materialName));
        Assert.Contains("HeatNumber cannot exceed", exception.Message);
        Assert.Contains($"{MaxLength} characters", exception.Message);
    }

    [Fact]
    public void Constructor_WhenMaterialNameIsNull_ThrowsDomainException()
    {
        // Arrange
        var heatNumber = ValidHeatNumber;
        string? materialName = null;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Material(heatNumber, materialName!));
        Assert.Contains("MaterialName cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenMaterialNameIsEmpty_ThrowsDomainException()
    {
        // Arrange
        var heatNumber = ValidHeatNumber;
        var materialName = string.Empty;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Material(heatNumber, materialName));
        Assert.Contains("MaterialName cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenMaterialNameIsWhitespace_ThrowsDomainException()
    {
        // Arrange
        var heatNumber = ValidHeatNumber;
        var materialName = "   ";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Material(heatNumber, materialName));
        Assert.Contains("MaterialName cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenMaterialNameExceedsMaxLength_ThrowsDomainException()
    {
        // Arrange
        var heatNumber = ValidHeatNumber;
        var materialName = new string('B', MaxLength + 1);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Material(heatNumber, materialName));
        Assert.Contains("MaterialName cannot exceed", exception.Message);
        Assert.Contains($"{MaxLength} characters", exception.Message);
    }

    [Fact]
    public void Constructor_WhenValidParameters_CreatesMaterial()
    {
        // Arrange
        var heatNumber = ValidHeatNumber;
        var materialName = ValidMaterialName;

        // Act
        var material = new Material(heatNumber, materialName);

        // Assert
        Assert.NotNull(material);
        Assert.Equal(heatNumber, material.HeatNumber);
        Assert.Equal(materialName, material.MaterialName);
        Assert.NotNull(material.StockLots);
        Assert.Empty(material.StockLots);
        Assert.Null(material.InactivatedDateTime);
    }

    [Fact]
    public void Constructor_WhenHeatNumberIsMaxLength_CreatesMaterial()
    {
        // Arrange
        var heatNumber = new string('A', MaxLength);
        var materialName = ValidMaterialName;

        // Act
        var material = new Material(heatNumber, materialName);

        // Assert
        Assert.NotNull(material);
        Assert.Equal(heatNumber, material.HeatNumber);
    }

    [Fact]
    public void Constructor_WhenMaterialNameIsMaxLength_CreatesMaterial()
    {
        // Arrange
        var heatNumber = ValidHeatNumber;
        var materialName = new string('B', MaxLength);

        // Act
        var material = new Material(heatNumber, materialName);

        // Assert
        Assert.NotNull(material);
        Assert.Equal(materialName, material.MaterialName);
    }

    #endregion

    #region Property Setter Tests

    [Fact]
    public void HeatNumberSetter_WhenValueIsNull_ThrowsDomainException()
    {
        // Arrange
        var material = new Material(ValidHeatNumber, ValidMaterialName);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => material.HeatNumber = null!);
        Assert.Contains("HeatNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void HeatNumberSetter_WhenValueIsEmpty_ThrowsDomainException()
    {
        // Arrange
        var material = new Material(ValidHeatNumber, ValidMaterialName);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => material.HeatNumber = string.Empty);
        Assert.Contains("HeatNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void HeatNumberSetter_WhenValueIsWhitespace_ThrowsDomainException()
    {
        // Arrange
        var material = new Material(ValidHeatNumber, ValidMaterialName);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => material.HeatNumber = "   ");
        Assert.Contains("HeatNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void HeatNumberSetter_WhenValueExceedsMaxLength_ThrowsDomainException()
    {
        // Arrange
        var material = new Material(ValidHeatNumber, ValidMaterialName);
        var invalidValue = new string('A', MaxLength + 1);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => material.HeatNumber = invalidValue);
        Assert.Contains("HeatNumber cannot exceed", exception.Message);
    }

    [Fact]
    public void HeatNumberSetter_WhenValueIsValid_UpdatesProperty()
    {
        // Arrange
        var material = new Material(ValidHeatNumber, ValidMaterialName);
        var newHeatNumber = "HN999999";

        // Act
        material.HeatNumber = newHeatNumber;

        // Assert
        Assert.Equal(newHeatNumber, material.HeatNumber);
    }

    [Fact]
    public void MaterialNameSetter_WhenValueIsNull_ThrowsDomainException()
    {
        // Arrange
        var material = new Material(ValidHeatNumber, ValidMaterialName);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => material.MaterialName = null!);
        Assert.Contains("MaterialName cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void MaterialNameSetter_WhenValueIsEmpty_ThrowsDomainException()
    {
        // Arrange
        var material = new Material(ValidHeatNumber, ValidMaterialName);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => material.MaterialName = string.Empty);
        Assert.Contains("MaterialName cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void MaterialNameSetter_WhenValueIsWhitespace_ThrowsDomainException()
    {
        // Arrange
        var material = new Material(ValidHeatNumber, ValidMaterialName);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => material.MaterialName = "   ");
        Assert.Contains("MaterialName cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void MaterialNameSetter_WhenValueExceedsMaxLength_ThrowsDomainException()
    {
        // Arrange
        var material = new Material(ValidHeatNumber, ValidMaterialName);
        var invalidValue = new string('B', MaxLength + 1);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => material.MaterialName = invalidValue);
        Assert.Contains("MaterialName cannot exceed", exception.Message);
    }

    [Fact]
    public void MaterialNameSetter_WhenValueIsValid_UpdatesProperty()
    {
        // Arrange
        var material = new Material(ValidHeatNumber, ValidMaterialName);
        var newMaterialName = "Steel-B2";

        // Act
        material.MaterialName = newMaterialName;

        // Assert
        Assert.Equal(newMaterialName, material.MaterialName);
    }

    #endregion

    #region Method Tests

    [Fact]
    public void Inactivate_WhenMaterialIsActive_SetsInactivatedDateTime()
    {
        // Arrange
        var material = new Material(ValidHeatNumber, ValidMaterialName);
        Assert.Null(material.InactivatedDateTime);

        // Act
        material.Inactivate();

        // Assert
        Assert.NotNull(material.InactivatedDateTime);
        Assert.True(material.InactivatedDateTime.Value <= DateTimeOffset.UtcNow);
        Assert.True(material.InactivatedDateTime.Value >= DateTimeOffset.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void Inactivate_WhenMaterialIsActive_SetsInactivatedByUserId()
    {
        // Arrange
        var material = new Material(ValidHeatNumber, ValidMaterialName);
        var userId = 42;

        // Act
        material.Inactivate(userId);

        // Assert
        Assert.NotNull(material.InactivatedDateTime);
        Assert.Equal(userId, material.InactivatedByUserId);
    }

    [Fact]
    public void Inactivate_WhenMaterialIsActive_WithNullUserId_SetsInactivatedDateTime()
    {
        // Arrange
        var material = new Material(ValidHeatNumber, ValidMaterialName);

        // Act
        material.Inactivate(null);

        // Assert
        Assert.NotNull(material.InactivatedDateTime);
        Assert.Null(material.InactivatedByUserId);
    }

    [Fact]
    public void Inactivate_WhenMaterialIsAlreadyInactivated_ThrowsDomainException()
    {
        // Arrange
        var material = new Material(ValidHeatNumber, ValidMaterialName);
        material.Inactivate();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => material.Inactivate());
        Assert.Contains("already inactivated", exception.Message);
        Assert.Contains("cannot be inactivated again", exception.Message);
    }

    [Fact]
    public void Inactivate_WhenMaterialIsAlreadyInactivated_WithUserId_ThrowsDomainException()
    {
        // Arrange
        var material = new Material(ValidHeatNumber, ValidMaterialName);
        material.Inactivate(1);
        var originalInactivatedDateTime = material.InactivatedDateTime;
        var originalInactivatedByUserId = material.InactivatedByUserId;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => material.Inactivate(2));
        Assert.Contains("already inactivated", exception.Message);

        // Assert that state was not changed
        Assert.Equal(originalInactivatedDateTime, material.InactivatedDateTime);
        Assert.Equal(originalInactivatedByUserId, material.InactivatedByUserId);
    }

    #endregion
}

