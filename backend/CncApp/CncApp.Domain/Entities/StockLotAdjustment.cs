using CncApp.Domain.Common;
using CncApp.Domain.Enums;

namespace CncApp.Domain.Entities;

public class StockLotAdjustment : AuditableEntityBase
{
    private const int MaxNotesLength = 2000;

    // Private constructor for EF Core
    // Sets backing fields directly to avoid validation during materialization
    private StockLotAdjustment()
    {
        _notes = null;
        StockLot = null!;
    }

    /// <summary>
    /// Creates a new StockLotAdjustment instance with validated invariants.
    /// </summary>
    /// <param name="stockLotId">The stock lot ID (required, must be greater than 0).</param>
    /// <param name="deltaBars">The delta bars value (required).</param>
    /// <param name="reason">The adjustment reason (required).</param>
    /// <param name="jobId">The job ID (optional).</param>
    /// <param name="notes">The notes (optional, max 2000 characters).</param>
    /// <exception cref="DomainException">Thrown when invariants are violated.</exception>
    public StockLotAdjustment(int stockLotId, int deltaBars, StockLotAdjustmentReasonEnum reason, int? jobId = null, string? notes = null)
    {
        StockLotId = stockLotId;
        DeltaBars = deltaBars;
        Reason = reason;
        JobId = jobId;
        Notes = notes;
        StockLot = null!;
    }

    private int _stockLotId;

    public int StockLotId
    {
        get => _stockLotId;
        set
        {
            if (value <= 0)
            {
                throw new DomainException("StockLotId must be greater than 0.");
            }
            _stockLotId = value;
        }
    }

    public int? JobId { get; set; }

    public int DeltaBars { get; set; }

    public StockLotAdjustmentReasonEnum Reason { get; set; }

    private string? _notes;

    public string? Notes
    {
        get => _notes;
        set
        {
            Guard.AgainstMaxLength(value, MaxNotesLength, nameof(Notes));
            _notes = value;
        }
    }

    public StockLot StockLot { get; set; } = null!;

    /// <summary>
    /// Inactivates the stock lot adjustment (soft-delete).
    /// Prevents double-inactivation by throwing a DomainException if already inactivated.
    /// </summary>
    /// <param name="inactivatedByUserId">The ID of the user performing the inactivation (optional).</param>
    /// <exception cref="DomainException">Thrown when the stock lot adjustment is already inactivated.</exception>
    public void Inactivate(int? inactivatedByUserId = null)
    {
        if (InactivatedDateTime.HasValue)
        {
            throw new DomainException("StockLotAdjustment is already inactivated and cannot be inactivated again.");
        }

        InactivatedDateTime = DateTimeOffset.UtcNow;
        InactivatedByUserId = inactivatedByUserId;
    }
}
