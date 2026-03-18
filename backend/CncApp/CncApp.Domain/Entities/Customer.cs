using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class Customer : AuditableEntityBase
{
    private const int MaxCompanyNameLength = 100;
    private const int MaxPhoneLength = 20;
    private const int MaxEmailLength = 150;
    private const int MaxAddressLength = 200;

    // Private constructor for EF Core
    // Sets backing fields directly to avoid validation during materialization
    private Customer()
    {
        _companyName = string.Empty;
        _phone = string.Empty;
        _email = string.Empty;
        _address = string.Empty;
        Orders = new List<Order>();
    }

    /// <summary>
    /// Creates a new Customer instance with validated invariants.
    /// </summary>
    /// <param name="companyName">The company name (required, max 100 characters).</param>
    /// <param name="phone">The phone number (required, max 20 characters).</param>
    /// <param name="email">The email address (required, max 150 characters).</param>
    /// <param name="address">The address (required, max 200 characters).</param>
    /// <exception cref="DomainException">Thrown when invariants are violated.</exception>
    public Customer(string companyName, string phone, string email, string address)
    {
        CompanyName = companyName;
        Phone = phone;
        Email = email;
        Address = address;
        Orders = new List<Order>();
    }

    private string _companyName = string.Empty;

    public string CompanyName
    {
        get => _companyName;
        set
        {
            Guard.AgainstNullOrWhiteSpace(value, nameof(CompanyName));
            Guard.AgainstMaxLength(value, MaxCompanyNameLength, nameof(CompanyName));
            _companyName = value;
        }
    }

    private string _phone = string.Empty;

    public string Phone
    {
        get => _phone;
        set
        {
            Guard.AgainstNullOrWhiteSpace(value, nameof(Phone));
            Guard.AgainstMaxLength(value, MaxPhoneLength, nameof(Phone));
            _phone = value;
        }
    }

    private string _email = string.Empty;

    public string Email
    {
        get => _email;
        set
        {
            Guard.AgainstNullOrWhiteSpace(value, nameof(Email));
            Guard.AgainstMaxLength(value, MaxEmailLength, nameof(Email));
            _email = value;
        }
    }

    private string _address = string.Empty;

    public string Address
    {
        get => _address;
        set
        {
            Guard.AgainstNullOrWhiteSpace(value, nameof(Address));
            Guard.AgainstMaxLength(value, MaxAddressLength, nameof(Address));
            _address = value;
        }
    }

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    /// <summary>
    /// Inactivates the customer (soft-delete).
    /// Prevents double-inactivation by throwing a DomainException if already inactivated.
    /// </summary>
    /// <param name="inactivatedByUserId">The ID of the user performing the inactivation (optional).</param>
    /// <exception cref="DomainException">Thrown when the customer is already inactivated.</exception>
    public void Inactivate(int? inactivatedByUserId = null)
    {
        if (InactivatedDateTime.HasValue)
        {
            throw new DomainException("Customer is already inactivated and cannot be inactivated again.");
        }

        InactivatedDateTime = DateTimeOffset.UtcNow;
        InactivatedByUserId = inactivatedByUserId;
    }
}
