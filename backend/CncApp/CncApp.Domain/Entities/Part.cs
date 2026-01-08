using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class Part : AuditableEntityBase
{
    // Private constructor for EF Core
    // Sets backing fields directly to avoid validation during materialization
    private Part()
    {
        Orders = new List<Order>();
    }

    /// <summary>
    /// Creates a new Part instance with validated invariants.
    /// </summary>
    /// <param name="approxPartCycleTime">The approximate part cycle time (required, must be non-negative).</param>
    /// <param name="checkPerPart">The check per part count (required, must be non-negative).</param>
    /// <exception cref="DomainException">Thrown when invariants are violated.</exception>
    public Part(TimeSpan approxPartCycleTime, int checkPerPart)
    {
        ApproxPartCycleTime = approxPartCycleTime;
        CheckPerPart = checkPerPart;
        Orders = new List<Order>();
    }

    private TimeSpan _approxPartCycleTime;

    public TimeSpan ApproxPartCycleTime
    {
        get => _approxPartCycleTime;
        set
        {
            if (value < TimeSpan.Zero)
            {
                throw new DomainException("ApproxPartCycleTime must be non-negative.");
            }
            _approxPartCycleTime = value;
        }
    }

    private int _checkPerPart;

    public int CheckPerPart
    {
        get => _checkPerPart;
        set
        {
            if (value < 0)
            {
                throw new DomainException("CheckPerPart must be non-negative.");
            }
            _checkPerPart = value;
        }
    }

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    /// <summary>
    /// Inactivates the part (soft-delete).
    /// Prevents double-inactivation by throwing a DomainException if already inactivated.
    /// </summary>
    /// <param name="inactivatedByUserId">The ID of the user performing the inactivation (optional).</param>
    /// <exception cref="DomainException">Thrown when the part is already inactivated.</exception>
    public void Inactivate(int? inactivatedByUserId = null)
    {
        if (InactivatedDateTime.HasValue)
        {
            throw new DomainException("Part is already inactivated and cannot be inactivated again.");
        }

        InactivatedDateTime = DateTimeOffset.UtcNow;
        InactivatedByUserId = inactivatedByUserId;
    }
}
