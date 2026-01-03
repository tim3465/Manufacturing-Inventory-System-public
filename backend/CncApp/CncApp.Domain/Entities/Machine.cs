using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class Machine : AuditableEntityBase
{
    private const int MaxSerialNumberLength = 100;
    private const int MaxModelNumberLength = 100;

    // Private constructor for EF Core
    // Sets backing fields directly to avoid validation during materialization
    private Machine()
    {
        _serialNumber = string.Empty;
        _modelNumber = string.Empty;
        Jobs = new List<Job>();
    }

    /// <summary>
    /// Creates a new Machine instance with validated invariants.
    /// </summary>
    /// <param name="serialNumber">The machine serial number (required, max 100 characters).</param>
    /// <param name="modelNumber">The machine model number (required, max 100 characters).</param>
    /// <exception cref="DomainException">Thrown when invariants are violated.</exception>
    public Machine(string serialNumber, string modelNumber)
    {
        SerialNumber = serialNumber;
        ModelNumber = modelNumber;
        Jobs = new List<Job>();
    }

    private string _serialNumber = string.Empty;

    public string SerialNumber
    {
        get => _serialNumber;
        set
        {
            Guard.AgainstNullOrWhiteSpace(value, nameof(SerialNumber));
            Guard.AgainstMaxLength(value, MaxSerialNumberLength, nameof(SerialNumber));
            _serialNumber = value;
        }
    }

    private string _modelNumber = string.Empty;

    public string ModelNumber
    {
        get => _modelNumber;
        set
        {
            Guard.AgainstNullOrWhiteSpace(value, nameof(ModelNumber));
            Guard.AgainstMaxLength(value, MaxModelNumberLength, nameof(ModelNumber));
            _modelNumber = value;
        }
    }

    public ICollection<Job> Jobs { get; set; } = new List<Job>();

    /// <summary>
    /// Inactivates the machine (soft-delete).
    /// Prevents double-inactivation by throwing a DomainException if already inactivated.
    /// </summary>
    /// <param name="inactivatedByUserId">The ID of the user performing the inactivation (optional).</param>
    /// <exception cref="DomainException">Thrown when the machine is already inactivated.</exception>
    public void Inactivate(int? inactivatedByUserId = null)
    {
        if (InactivatedDateTime.HasValue)
        {
            throw new DomainException("Machine is already inactivated and cannot be inactivated again.");
        }

        InactivatedDateTime = DateTimeOffset.UtcNow;
        InactivatedByUserId = inactivatedByUserId;
    }
}
