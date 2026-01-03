using CncApp.Domain.Common;
using CncApp.Domain.Entities;

namespace CncApp.Domain.Tests.Entities;

/// <summary>
/// Domain tests for Machine entity invariants.
/// Tests verify that invalid states cannot be created and that DomainException is thrown for violations.
/// These tests do NOT access the database or test application workflows.
/// </summary>
public class MachineTests
{
    private const string ValidSerialNumber = "SN123456";
    private const string ValidModelNumber = "MODEL-X1";
    private const int MaxLength = 100;

    #region Constructor Tests

    [Fact]
    public void Constructor_WhenSerialNumberIsNull_ThrowsDomainException()
    {
        // Arrange
        string? serialNumber = null;
        var modelNumber = ValidModelNumber;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Machine(serialNumber!, modelNumber));
        Assert.Contains("SerialNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenSerialNumberIsEmpty_ThrowsDomainException()
    {
        // Arrange
        var serialNumber = string.Empty;
        var modelNumber = ValidModelNumber;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Machine(serialNumber, modelNumber));
        Assert.Contains("SerialNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenSerialNumberIsWhitespace_ThrowsDomainException()
    {
        // Arrange
        var serialNumber = "   ";
        var modelNumber = ValidModelNumber;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Machine(serialNumber, modelNumber));
        Assert.Contains("SerialNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenSerialNumberExceedsMaxLength_ThrowsDomainException()
    {
        // Arrange
        var serialNumber = new string('A', MaxLength + 1);
        var modelNumber = ValidModelNumber;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Machine(serialNumber, modelNumber));
        Assert.Contains("SerialNumber cannot exceed", exception.Message);
        Assert.Contains($"{MaxLength} characters", exception.Message);
    }

    [Fact]
    public void Constructor_WhenModelNumberIsNull_ThrowsDomainException()
    {
        // Arrange
        var serialNumber = ValidSerialNumber;
        string? modelNumber = null;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Machine(serialNumber, modelNumber!));
        Assert.Contains("ModelNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenModelNumberIsEmpty_ThrowsDomainException()
    {
        // Arrange
        var serialNumber = ValidSerialNumber;
        var modelNumber = string.Empty;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Machine(serialNumber, modelNumber));
        Assert.Contains("ModelNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenModelNumberIsWhitespace_ThrowsDomainException()
    {
        // Arrange
        var serialNumber = ValidSerialNumber;
        var modelNumber = "   ";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Machine(serialNumber, modelNumber));
        Assert.Contains("ModelNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WhenModelNumberExceedsMaxLength_ThrowsDomainException()
    {
        // Arrange
        var serialNumber = ValidSerialNumber;
        var modelNumber = new string('B', MaxLength + 1);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Machine(serialNumber, modelNumber));
        Assert.Contains("ModelNumber cannot exceed", exception.Message);
        Assert.Contains($"{MaxLength} characters", exception.Message);
    }

    [Fact]
    public void Constructor_WhenValidParameters_CreatesMachine()
    {
        // Arrange
        var serialNumber = ValidSerialNumber;
        var modelNumber = ValidModelNumber;

        // Act
        var machine = new Machine(serialNumber, modelNumber);

        // Assert
        Assert.NotNull(machine);
        Assert.Equal(serialNumber, machine.SerialNumber);
        Assert.Equal(modelNumber, machine.ModelNumber);
        Assert.NotNull(machine.Jobs);
        Assert.Empty(machine.Jobs);
        Assert.Null(machine.InactivatedDateTime);
    }

    [Fact]
    public void Constructor_WhenSerialNumberIsMaxLength_CreatesMachine()
    {
        // Arrange
        var serialNumber = new string('A', MaxLength);
        var modelNumber = ValidModelNumber;

        // Act
        var machine = new Machine(serialNumber, modelNumber);

        // Assert
        Assert.NotNull(machine);
        Assert.Equal(serialNumber, machine.SerialNumber);
    }

    [Fact]
    public void Constructor_WhenModelNumberIsMaxLength_CreatesMachine()
    {
        // Arrange
        var serialNumber = ValidSerialNumber;
        var modelNumber = new string('B', MaxLength);

        // Act
        var machine = new Machine(serialNumber, modelNumber);

        // Assert
        Assert.NotNull(machine);
        Assert.Equal(modelNumber, machine.ModelNumber);
    }

    #endregion

    #region Property Setter Tests

    [Fact]
    public void SerialNumberSetter_WhenValueIsNull_ThrowsDomainException()
    {
        // Arrange
        var machine = new Machine(ValidSerialNumber, ValidModelNumber);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => machine.SerialNumber = null!);
        Assert.Contains("SerialNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void SerialNumberSetter_WhenValueIsEmpty_ThrowsDomainException()
    {
        // Arrange
        var machine = new Machine(ValidSerialNumber, ValidModelNumber);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => machine.SerialNumber = string.Empty);
        Assert.Contains("SerialNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void SerialNumberSetter_WhenValueIsWhitespace_ThrowsDomainException()
    {
        // Arrange
        var machine = new Machine(ValidSerialNumber, ValidModelNumber);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => machine.SerialNumber = "   ");
        Assert.Contains("SerialNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void SerialNumberSetter_WhenValueExceedsMaxLength_ThrowsDomainException()
    {
        // Arrange
        var machine = new Machine(ValidSerialNumber, ValidModelNumber);
        var invalidValue = new string('A', MaxLength + 1);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => machine.SerialNumber = invalidValue);
        Assert.Contains("SerialNumber cannot exceed", exception.Message);
    }

    [Fact]
    public void SerialNumberSetter_WhenValueIsValid_UpdatesProperty()
    {
        // Arrange
        var machine = new Machine(ValidSerialNumber, ValidModelNumber);
        var newSerialNumber = "SN999999";

        // Act
        machine.SerialNumber = newSerialNumber;

        // Assert
        Assert.Equal(newSerialNumber, machine.SerialNumber);
    }

    [Fact]
    public void ModelNumberSetter_WhenValueIsNull_ThrowsDomainException()
    {
        // Arrange
        var machine = new Machine(ValidSerialNumber, ValidModelNumber);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => machine.ModelNumber = null!);
        Assert.Contains("ModelNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void ModelNumberSetter_WhenValueIsEmpty_ThrowsDomainException()
    {
        // Arrange
        var machine = new Machine(ValidSerialNumber, ValidModelNumber);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => machine.ModelNumber = string.Empty);
        Assert.Contains("ModelNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void ModelNumberSetter_WhenValueIsWhitespace_ThrowsDomainException()
    {
        // Arrange
        var machine = new Machine(ValidSerialNumber, ValidModelNumber);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => machine.ModelNumber = "   ");
        Assert.Contains("ModelNumber cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void ModelNumberSetter_WhenValueExceedsMaxLength_ThrowsDomainException()
    {
        // Arrange
        var machine = new Machine(ValidSerialNumber, ValidModelNumber);
        var invalidValue = new string('B', MaxLength + 1);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => machine.ModelNumber = invalidValue);
        Assert.Contains("ModelNumber cannot exceed", exception.Message);
    }

    [Fact]
    public void ModelNumberSetter_WhenValueIsValid_UpdatesProperty()
    {
        // Arrange
        var machine = new Machine(ValidSerialNumber, ValidModelNumber);
        var newModelNumber = "MODEL-Y2";

        // Act
        machine.ModelNumber = newModelNumber;

        // Assert
        Assert.Equal(newModelNumber, machine.ModelNumber);
    }

    #endregion

    #region Inactivate Method Tests

    [Fact]
    public void Inactivate_WhenMachineIsActive_SetsInactivatedDateTime()
    {
        // Arrange
        var machine = new Machine(ValidSerialNumber, ValidModelNumber);
        Assert.Null(machine.InactivatedDateTime);

        // Act
        machine.Inactivate();

        // Assert
        Assert.NotNull(machine.InactivatedDateTime);
        Assert.True(machine.InactivatedDateTime.Value <= DateTimeOffset.UtcNow);
        Assert.True(machine.InactivatedDateTime.Value >= DateTimeOffset.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void Inactivate_WhenMachineIsActive_SetsInactivatedByUserId()
    {
        // Arrange
        var machine = new Machine(ValidSerialNumber, ValidModelNumber);
        var userId = 42;

        // Act
        machine.Inactivate(userId);

        // Assert
        Assert.NotNull(machine.InactivatedDateTime);
        Assert.Equal(userId, machine.InactivatedByUserId);
    }

    [Fact]
    public void Inactivate_WhenMachineIsActive_WithNullUserId_SetsInactivatedDateTime()
    {
        // Arrange
        var machine = new Machine(ValidSerialNumber, ValidModelNumber);

        // Act
        machine.Inactivate(null);

        // Assert
        Assert.NotNull(machine.InactivatedDateTime);
        Assert.Null(machine.InactivatedByUserId);
    }

    [Fact]
    public void Inactivate_WhenMachineIsAlreadyInactivated_ThrowsDomainException()
    {
        // Arrange
        var machine = new Machine(ValidSerialNumber, ValidModelNumber);
        machine.Inactivate();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => machine.Inactivate());
        Assert.Contains("already inactivated", exception.Message);
        Assert.Contains("cannot be inactivated again", exception.Message);
    }

    [Fact]
    public void Inactivate_WhenMachineIsAlreadyInactivated_WithUserId_ThrowsDomainException()
    {
        // Arrange
        var machine = new Machine(ValidSerialNumber, ValidModelNumber);
        machine.Inactivate(1);
        var originalInactivatedDateTime = machine.InactivatedDateTime;
        var originalInactivatedByUserId = machine.InactivatedByUserId;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => machine.Inactivate(2));
        Assert.Contains("already inactivated", exception.Message);

        // Assert that state was not changed
        Assert.Equal(originalInactivatedDateTime, machine.InactivatedDateTime);
        Assert.Equal(originalInactivatedByUserId, machine.InactivatedByUserId);
    }

    #endregion
}

