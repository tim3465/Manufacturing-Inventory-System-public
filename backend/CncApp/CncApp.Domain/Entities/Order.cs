using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class Order : AuditableEntityBase
{
    // Private constructor for EF Core
    // Sets backing fields directly to avoid validation during materialization
    private Order()
    {
        Part = null!;
        Customer = null!;
        Jobs = new List<Job>();
    }

    /// <summary>
    /// Creates a new Order instance with validated invariants.
    /// </summary>
    /// <param name="partId">The part ID (required, must be positive).</param>
    /// <param name="customerId">The customer ID (required, must be positive).</param>
    /// <param name="partAmountRequested">The part amount requested (required, must be positive).</param>
    /// <param name="partsPerBar">The parts per bar (optional, must be non-negative).</param>
    /// <exception cref="DomainException">Thrown when invariants are violated.</exception>
    public Order(int partId, int customerId, int partAmountRequested, int partsPerBar = 0)
    {
        PartId = partId;
        CustomerId = customerId;
        PartAmountRequested = partAmountRequested;
        PartsPerBar = partsPerBar;
        Part = null!;
        Customer = null!;
        Jobs = new List<Job>();
    }

    private int _partId;

    public int PartId
    {
        get => _partId;
        set
        {
            if (value <= 0)
            {
                throw new DomainException("PartId must be positive.");
            }
            _partId = value;
        }
    }

    private int _customerId;

    public int CustomerId
    {
        get => _customerId;
        set
        {
            if (value <= 0)
            {
                throw new DomainException("CustomerId must be positive.");
            }
            _customerId = value;
        }
    }

    private int _partAmountRequested;

    public int PartAmountRequested
    {
        get => _partAmountRequested;
        set
        {
            if (value <= 0)
            {
                throw new DomainException("PartAmountRequested must be positive.");
            }
            _partAmountRequested = value;
        }
    }

    private int _partsPerBar;

    public int PartsPerBar
    {
        get => _partsPerBar;
        set
        {
            if (value < 0)
            {
                throw new DomainException("PartsPerBar must be non-negative.");
            }
            _partsPerBar = value;
        }
    }

    public Part Part { get; set; } = null!;

    public Customer Customer { get; set; } = null!;

    public ICollection<Job> Jobs { get; set; } = new List<Job>();

    /// <summary>
    /// Inactivates the order (soft-delete).
    /// Prevents double-inactivation by throwing a DomainException if already inactivated.
    /// </summary>
    /// <param name="inactivatedByUserId">The ID of the user performing the inactivation (optional).</param>
    /// <exception cref="DomainException">Thrown when the order is already inactivated.</exception>
    public void Inactivate(int? inactivatedByUserId = null)
    {
        if (InactivatedDateTime.HasValue)
        {
            throw new DomainException("Order is already inactivated and cannot be inactivated again.");
        }

        InactivatedDateTime = DateTimeOffset.UtcNow;
        InactivatedByUserId = inactivatedByUserId;
    }
}
