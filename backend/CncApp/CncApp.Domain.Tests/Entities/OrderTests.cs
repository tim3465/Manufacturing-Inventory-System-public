using CncApp.Domain.Common;
using CncApp.Domain.Entities;

namespace CncApp.Domain.Tests.Entities;

/// <summary>
/// Domain tests for Order entity invariants.
/// Tests verify that invalid states cannot be created and that DomainException is thrown for violations.
/// These tests do NOT access the database or test application workflows.
/// </summary>
public class OrderTests
{
    private const int ValidPartId = 1;
    private const int ValidCustomerId = 1;
    private const int ValidPartAmountRequested = 100;
    private const int ValidPartsPerBar = 10;

    #region Constructor Tests

    [Fact]
    public void Constructor_WhenPartIdIsZero_ThrowsDomainException()
    {
        // Arrange
        var partId = 0;
        var customerId = ValidCustomerId;
        var partAmountRequested = ValidPartAmountRequested;
        var partsPerBar = ValidPartsPerBar;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Order(partId, customerId, partAmountRequested, partsPerBar));
        Assert.Contains("PartId must be positive", exception.Message);
    }

    [Fact]
    public void Constructor_WhenPartIdIsNegative_ThrowsDomainException()
    {
        // Arrange
        var partId = -1;
        var customerId = ValidCustomerId;
        var partAmountRequested = ValidPartAmountRequested;
        var partsPerBar = ValidPartsPerBar;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Order(partId, customerId, partAmountRequested, partsPerBar));
        Assert.Contains("PartId must be positive", exception.Message);
    }

    [Fact]
    public void Constructor_WhenCustomerIdIsZero_ThrowsDomainException()
    {
        // Arrange
        var partId = ValidPartId;
        var customerId = 0;
        var partAmountRequested = ValidPartAmountRequested;
        var partsPerBar = ValidPartsPerBar;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Order(partId, customerId, partAmountRequested, partsPerBar));
        Assert.Contains("CustomerId must be positive", exception.Message);
    }

    [Fact]
    public void Constructor_WhenCustomerIdIsNegative_ThrowsDomainException()
    {
        // Arrange
        var partId = ValidPartId;
        var customerId = -1;
        var partAmountRequested = ValidPartAmountRequested;
        var partsPerBar = ValidPartsPerBar;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Order(partId, customerId, partAmountRequested, partsPerBar));
        Assert.Contains("CustomerId must be positive", exception.Message);
    }

    [Fact]
    public void Constructor_WhenPartAmountRequestedIsZero_ThrowsDomainException()
    {
        // Arrange
        var partId = ValidPartId;
        var customerId = ValidCustomerId;
        var partAmountRequested = 0;
        var partsPerBar = ValidPartsPerBar;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Order(partId, customerId, partAmountRequested, partsPerBar));
        Assert.Contains("PartAmountRequested must be positive", exception.Message);
    }

    [Fact]
    public void Constructor_WhenPartAmountRequestedIsNegative_ThrowsDomainException()
    {
        // Arrange
        var partId = ValidPartId;
        var customerId = ValidCustomerId;
        var partAmountRequested = -1;
        var partsPerBar = ValidPartsPerBar;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Order(partId, customerId, partAmountRequested, partsPerBar));
        Assert.Contains("PartAmountRequested must be positive", exception.Message);
    }

    [Fact]
    public void Constructor_WhenPartsPerBarIsNegative_ThrowsDomainException()
    {
        // Arrange
        var partId = ValidPartId;
        var customerId = ValidCustomerId;
        var partAmountRequested = ValidPartAmountRequested;
        var partsPerBar = -1;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Order(partId, customerId, partAmountRequested, partsPerBar));
        Assert.Contains("PartsPerBar must be non-negative", exception.Message);
    }

    [Fact]
    public void Constructor_WhenValidParameters_CreatesOrder()
    {
        // Arrange
        var partId = ValidPartId;
        var customerId = ValidCustomerId;
        var partAmountRequested = ValidPartAmountRequested;
        var partsPerBar = ValidPartsPerBar;

        // Act
        var order = new Order(partId, customerId, partAmountRequested, partsPerBar);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(partId, order.PartId);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(partAmountRequested, order.PartAmountRequested);
        Assert.Equal(partsPerBar, order.PartsPerBar);
        Assert.NotNull(order.Jobs);
        Assert.Empty(order.Jobs);
        Assert.Null(order.InactivatedDateTime);
    }

    [Fact]
    public void Constructor_WhenPartsPerBarIsZero_CreatesOrder()
    {
        // Arrange
        var partId = ValidPartId;
        var customerId = ValidCustomerId;
        var partAmountRequested = ValidPartAmountRequested;
        var partsPerBar = 0;

        // Act
        var order = new Order(partId, customerId, partAmountRequested, partsPerBar);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(0, order.PartsPerBar);
    }

    [Fact]
    public void Constructor_WhenPartsPerBarIsOmitted_UsesDefaultZero()
    {
        // Arrange
        var partId = ValidPartId;
        var customerId = ValidCustomerId;
        var partAmountRequested = ValidPartAmountRequested;

        // Act
        var order = new Order(partId, customerId, partAmountRequested);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(0, order.PartsPerBar);
    }

    #endregion

    #region Property Setter Tests

    [Fact]
    public void PartIdSetter_WhenValueIsZero_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => order.PartId = 0);
        Assert.Contains("PartId must be positive", exception.Message);
    }

    [Fact]
    public void PartIdSetter_WhenValueIsNegative_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => order.PartId = -1);
        Assert.Contains("PartId must be positive", exception.Message);
    }

    [Fact]
    public void PartIdSetter_WhenValueIsValid_UpdatesProperty()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);
        var newPartId = 2;

        // Act
        order.PartId = newPartId;

        // Assert
        Assert.Equal(newPartId, order.PartId);
    }

    [Fact]
    public void CustomerIdSetter_WhenValueIsZero_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => order.CustomerId = 0);
        Assert.Contains("CustomerId must be positive", exception.Message);
    }

    [Fact]
    public void CustomerIdSetter_WhenValueIsNegative_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => order.CustomerId = -1);
        Assert.Contains("CustomerId must be positive", exception.Message);
    }

    [Fact]
    public void CustomerIdSetter_WhenValueIsValid_UpdatesProperty()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);
        var newCustomerId = 2;

        // Act
        order.CustomerId = newCustomerId;

        // Assert
        Assert.Equal(newCustomerId, order.CustomerId);
    }

    [Fact]
    public void PartAmountRequestedSetter_WhenValueIsZero_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => order.PartAmountRequested = 0);
        Assert.Contains("PartAmountRequested must be positive", exception.Message);
    }

    [Fact]
    public void PartAmountRequestedSetter_WhenValueIsNegative_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => order.PartAmountRequested = -1);
        Assert.Contains("PartAmountRequested must be positive", exception.Message);
    }

    [Fact]
    public void PartAmountRequestedSetter_WhenValueIsValid_UpdatesProperty()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);
        var newPartAmountRequested = 200;

        // Act
        order.PartAmountRequested = newPartAmountRequested;

        // Assert
        Assert.Equal(newPartAmountRequested, order.PartAmountRequested);
    }

    [Fact]
    public void PartsPerBarSetter_WhenValueIsNegative_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => order.PartsPerBar = -1);
        Assert.Contains("PartsPerBar must be non-negative", exception.Message);
    }

    [Fact]
    public void PartsPerBarSetter_WhenValueIsValid_UpdatesProperty()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);
        var newPartsPerBar = 20;

        // Act
        order.PartsPerBar = newPartsPerBar;

        // Assert
        Assert.Equal(newPartsPerBar, order.PartsPerBar);
    }

    [Fact]
    public void PartsPerBarSetter_WhenValueIsZero_UpdatesProperty()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);

        // Act
        order.PartsPerBar = 0;

        // Assert
        Assert.Equal(0, order.PartsPerBar);
    }

    #endregion

    #region Inactivate Method Tests

    [Fact]
    public void Inactivate_WhenOrderIsActive_SetsInactivatedDateTime()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);
        Assert.Null(order.InactivatedDateTime);

        // Act
        order.Inactivate();

        // Assert
        Assert.NotNull(order.InactivatedDateTime);
        Assert.True(order.InactivatedDateTime.Value <= DateTimeOffset.UtcNow);
        Assert.True(order.InactivatedDateTime.Value >= DateTimeOffset.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void Inactivate_WhenOrderIsActive_SetsInactivatedByUserId()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);
        var userId = 42;

        // Act
        order.Inactivate(userId);

        // Assert
        Assert.NotNull(order.InactivatedDateTime);
        Assert.Equal(userId, order.InactivatedByUserId);
    }

    [Fact]
    public void Inactivate_WhenOrderIsActive_WithNullUserId_SetsInactivatedDateTime()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);

        // Act
        order.Inactivate(null);

        // Assert
        Assert.NotNull(order.InactivatedDateTime);
        Assert.Null(order.InactivatedByUserId);
    }

    [Fact]
    public void Inactivate_WhenOrderIsAlreadyInactivated_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);
        order.Inactivate();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => order.Inactivate());
        Assert.Contains("already inactivated", exception.Message);
        Assert.Contains("cannot be inactivated again", exception.Message);
    }

    [Fact]
    public void Inactivate_WhenOrderIsAlreadyInactivated_WithUserId_ThrowsDomainException()
    {
        // Arrange
        var order = new Order(ValidPartId, ValidCustomerId, ValidPartAmountRequested, ValidPartsPerBar);
        order.Inactivate(1);
        var originalInactivatedDateTime = order.InactivatedDateTime;
        var originalInactivatedByUserId = order.InactivatedByUserId;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => order.Inactivate(2));
        Assert.Contains("already inactivated", exception.Message);

        // Assert that state was not changed
        Assert.Equal(originalInactivatedDateTime, order.InactivatedDateTime);
        Assert.Equal(originalInactivatedByUserId, order.InactivatedByUserId);
    }

    #endregion
}

