using CncApp.Domain.Common;
using CncApp.Domain.Entities;

namespace CncApp.Domain.Tests.Entities;

/// <summary>
/// Domain tests for Customer entity invariants.
/// Tests verify that invalid states cannot be created and that DomainException is thrown for violations.
/// These tests do NOT access the database or test application workflows.
/// </summary>
public class CustomerTests
{
    private const string ValidCompanyName = "Acme Corp";
    private const string ValidPhone = "555-1234";
    private const string ValidEmail = "contact@acme.com";
    private const string ValidAddress = "123 Main St";

    #region Constructor Tests

    [Fact]
    public void Constructor_WhenCompanyNameIsNull_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Customer(null!, ValidPhone, ValidEmail, ValidAddress));
        Assert.Contains("CompanyName", exception.Message);
    }

    [Fact]
    public void Constructor_WhenCompanyNameIsWhiteSpace_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Customer("   ", ValidPhone, ValidEmail, ValidAddress));
        Assert.Contains("CompanyName", exception.Message);
    }

    [Fact]
    public void Constructor_WhenCompanyNameExceedsMaxLength_ThrowsDomainException()
    {
        var longName = new string('A', 101);
        var exception = Assert.Throws<DomainException>(() =>
            new Customer(longName, ValidPhone, ValidEmail, ValidAddress));
        Assert.Contains("CompanyName", exception.Message);
    }

    [Fact]
    public void Constructor_WhenPhoneIsNull_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Customer(ValidCompanyName, null!, ValidEmail, ValidAddress));
        Assert.Contains("Phone", exception.Message);
    }

    [Fact]
    public void Constructor_WhenPhoneIsWhiteSpace_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Customer(ValidCompanyName, "   ", ValidEmail, ValidAddress));
        Assert.Contains("Phone", exception.Message);
    }

    [Fact]
    public void Constructor_WhenPhoneExceedsMaxLength_ThrowsDomainException()
    {
        var longPhone = new string('1', 21);
        var exception = Assert.Throws<DomainException>(() =>
            new Customer(ValidCompanyName, longPhone, ValidEmail, ValidAddress));
        Assert.Contains("Phone", exception.Message);
    }

    [Fact]
    public void Constructor_WhenEmailIsNull_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Customer(ValidCompanyName, ValidPhone, null!, ValidAddress));
        Assert.Contains("Email", exception.Message);
    }

    [Fact]
    public void Constructor_WhenEmailIsWhiteSpace_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Customer(ValidCompanyName, ValidPhone, "   ", ValidAddress));
        Assert.Contains("Email", exception.Message);
    }

    [Fact]
    public void Constructor_WhenEmailExceedsMaxLength_ThrowsDomainException()
    {
        var longEmail = new string('a', 145) + "@b.com";
        var exception = Assert.Throws<DomainException>(() =>
            new Customer(ValidCompanyName, ValidPhone, longEmail, ValidAddress));
        Assert.Contains("Email", exception.Message);
    }

    [Fact]
    public void Constructor_WhenAddressIsNull_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Customer(ValidCompanyName, ValidPhone, ValidEmail, null!));
        Assert.Contains("Address", exception.Message);
    }

    [Fact]
    public void Constructor_WhenAddressIsWhiteSpace_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Customer(ValidCompanyName, ValidPhone, ValidEmail, "   "));
        Assert.Contains("Address", exception.Message);
    }

    [Fact]
    public void Constructor_WhenAddressExceedsMaxLength_ThrowsDomainException()
    {
        var longAddress = new string('X', 201);
        var exception = Assert.Throws<DomainException>(() =>
            new Customer(ValidCompanyName, ValidPhone, ValidEmail, longAddress));
        Assert.Contains("Address", exception.Message);
    }

    [Fact]
    public void Constructor_WhenValidParameters_CreatesCustomer()
    {
        var customer = new Customer(ValidCompanyName, ValidPhone, ValidEmail, ValidAddress);

        Assert.NotNull(customer);
        Assert.Equal(ValidCompanyName, customer.CompanyName);
        Assert.Equal(ValidPhone, customer.Phone);
        Assert.Equal(ValidEmail, customer.Email);
        Assert.Equal(ValidAddress, customer.Address);
        Assert.NotNull(customer.Orders);
        Assert.Empty(customer.Orders);
        Assert.Null(customer.InactivatedDateTime);
    }

    #endregion

    #region Property Setter Tests

    [Fact]
    public void CompanyNameSetter_WhenValueIsNull_ThrowsDomainException()
    {
        var customer = CreateValidCustomer();
        var exception = Assert.Throws<DomainException>(() => customer.CompanyName = null!);
        Assert.Contains("CompanyName", exception.Message);
    }

    [Fact]
    public void CompanyNameSetter_WhenValueIsWhiteSpace_ThrowsDomainException()
    {
        var customer = CreateValidCustomer();
        var exception = Assert.Throws<DomainException>(() => customer.CompanyName = "   ");
        Assert.Contains("CompanyName", exception.Message);
    }

    [Fact]
    public void CompanyNameSetter_WhenValueExceedsMaxLength_ThrowsDomainException()
    {
        var customer = CreateValidCustomer();
        var exception = Assert.Throws<DomainException>(() => customer.CompanyName = new string('A', 101));
        Assert.Contains("CompanyName", exception.Message);
    }

    [Fact]
    public void CompanyNameSetter_WhenValueIsValid_UpdatesProperty()
    {
        var customer = CreateValidCustomer();
        customer.CompanyName = "New Corp";
        Assert.Equal("New Corp", customer.CompanyName);
    }

    [Fact]
    public void PhoneSetter_WhenValueIsNull_ThrowsDomainException()
    {
        var customer = CreateValidCustomer();
        var exception = Assert.Throws<DomainException>(() => customer.Phone = null!);
        Assert.Contains("Phone", exception.Message);
    }

    [Fact]
    public void PhoneSetter_WhenValueExceedsMaxLength_ThrowsDomainException()
    {
        var customer = CreateValidCustomer();
        var exception = Assert.Throws<DomainException>(() => customer.Phone = new string('1', 21));
        Assert.Contains("Phone", exception.Message);
    }

    [Fact]
    public void PhoneSetter_WhenValueIsValid_UpdatesProperty()
    {
        var customer = CreateValidCustomer();
        customer.Phone = "999-8888";
        Assert.Equal("999-8888", customer.Phone);
    }

    [Fact]
    public void EmailSetter_WhenValueIsNull_ThrowsDomainException()
    {
        var customer = CreateValidCustomer();
        var exception = Assert.Throws<DomainException>(() => customer.Email = null!);
        Assert.Contains("Email", exception.Message);
    }

    [Fact]
    public void EmailSetter_WhenValueExceedsMaxLength_ThrowsDomainException()
    {
        var customer = CreateValidCustomer();
        var exception = Assert.Throws<DomainException>(() => customer.Email = new string('a', 145) + "@b.com");
        Assert.Contains("Email", exception.Message);
    }

    [Fact]
    public void EmailSetter_WhenValueIsValid_UpdatesProperty()
    {
        var customer = CreateValidCustomer();
        customer.Email = "new@email.com";
        Assert.Equal("new@email.com", customer.Email);
    }

    [Fact]
    public void AddressSetter_WhenValueIsNull_ThrowsDomainException()
    {
        var customer = CreateValidCustomer();
        var exception = Assert.Throws<DomainException>(() => customer.Address = null!);
        Assert.Contains("Address", exception.Message);
    }

    [Fact]
    public void AddressSetter_WhenValueExceedsMaxLength_ThrowsDomainException()
    {
        var customer = CreateValidCustomer();
        var exception = Assert.Throws<DomainException>(() => customer.Address = new string('X', 201));
        Assert.Contains("Address", exception.Message);
    }

    [Fact]
    public void AddressSetter_WhenValueIsValid_UpdatesProperty()
    {
        var customer = CreateValidCustomer();
        customer.Address = "456 Oak Ave";
        Assert.Equal("456 Oak Ave", customer.Address);
    }

    #endregion

    #region Inactivate Method Tests

    [Fact]
    public void Inactivate_WhenCustomerIsActive_SetsInactivatedDateTime()
    {
        var customer = CreateValidCustomer();
        Assert.Null(customer.InactivatedDateTime);

        customer.Inactivate();

        Assert.NotNull(customer.InactivatedDateTime);
        Assert.True(customer.InactivatedDateTime.Value <= DateTimeOffset.UtcNow);
        Assert.True(customer.InactivatedDateTime.Value >= DateTimeOffset.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void Inactivate_WhenCustomerIsActive_SetsInactivatedByUserId()
    {
        var customer = CreateValidCustomer();
        var userId = 42;

        customer.Inactivate(userId);

        Assert.NotNull(customer.InactivatedDateTime);
        Assert.Equal(userId, customer.InactivatedByUserId);
    }

    [Fact]
    public void Inactivate_WhenCustomerIsActive_WithNullUserId_SetsInactivatedDateTime()
    {
        var customer = CreateValidCustomer();

        customer.Inactivate(null);

        Assert.NotNull(customer.InactivatedDateTime);
        Assert.Null(customer.InactivatedByUserId);
    }

    [Fact]
    public void Inactivate_WhenCustomerIsAlreadyInactivated_ThrowsDomainException()
    {
        var customer = CreateValidCustomer();
        customer.Inactivate();

        var exception = Assert.Throws<DomainException>(() => customer.Inactivate());
        Assert.Contains("already inactivated", exception.Message);
        Assert.Contains("cannot be inactivated again", exception.Message);
    }

    [Fact]
    public void Inactivate_WhenCustomerIsAlreadyInactivated_WithUserId_ThrowsDomainException()
    {
        var customer = CreateValidCustomer();
        customer.Inactivate(1);
        var originalInactivatedDateTime = customer.InactivatedDateTime;
        var originalInactivatedByUserId = customer.InactivatedByUserId;

        var exception = Assert.Throws<DomainException>(() => customer.Inactivate(2));
        Assert.Contains("already inactivated", exception.Message);

        // Assert that state was not changed
        Assert.Equal(originalInactivatedDateTime, customer.InactivatedDateTime);
        Assert.Equal(originalInactivatedByUserId, customer.InactivatedByUserId);
    }

    #endregion

    private static Customer CreateValidCustomer() =>
        new(ValidCompanyName, ValidPhone, ValidEmail, ValidAddress);
}
