using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class User : AuditableEntityBase
{
    private const int MaxUserNameLength = 200;
    private const int MaxFirstNameLength = 200;
    private const int MaxLastNameLength = 200;

    private int _identityUserId;

    public int IdentityUserId
    {
        get => _identityUserId;
        set
        {
            if (value <= 0)
            {
                throw new DomainException("IdentityUserId must be greater than zero.");
            }

            _identityUserId = value;
        }
    }

    private string _userName = string.Empty;

    public string UserName
    {
        get => _userName;
        set
        {
            Guard.AgainstNullOrWhiteSpace(value, nameof(UserName));
            Guard.AgainstMaxLength(value, MaxUserNameLength, nameof(UserName));
            _userName = value;
        }
    }

    private string? _firstName;

    public string? FirstName
    {
        get => _firstName;
        set
        {
            if (value != null)
            {
                Guard.AgainstMaxLength(value, MaxFirstNameLength, nameof(FirstName));
            }

            _firstName = value;
        }
    }

    private string? _lastName;

    public string? LastName
    {
        get => _lastName;
        set
        {
            if (value != null)
            {
                Guard.AgainstMaxLength(value, MaxLastNameLength, nameof(LastName));
            }

            _lastName = value;
        }
    }

    // Email is NOT stored in Domain User - Identity owns email as source of truth
    // To get email, resolve via Identity using IdentityUserId

    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    /// <summary>
    /// Inactivates the user (soft-delete).
    /// Prevents double-inactivation by throwing a DomainException if already inactivated.
    /// </summary>
    /// <param name="inactivatedByUserId">The ID of the user performing the inactivation (optional).</param>
    /// <exception cref="DomainException">Thrown when the user is already inactivated.</exception>
    public void Inactivate(int? inactivatedByUserId = null)
    {
        if (InactivatedDateTime.HasValue)
        {
            throw new DomainException("User is already inactivated and cannot be inactivated again.");
        }

        InactivatedDateTime = DateTimeOffset.UtcNow;
        InactivatedByUserId = inactivatedByUserId;
    }
}
