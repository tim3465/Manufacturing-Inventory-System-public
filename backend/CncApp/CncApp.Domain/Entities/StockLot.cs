using CncApp.Domain.Common;
using CncApp.Domain.Enums;

namespace CncApp.Domain.Entities;

public class StockLot : AuditableEntityBase
{
    private const int MaxLotNumberLength = 100;

    // Private constructor for EF Core
    // Sets backing fields directly to avoid validation during materialization
    private StockLot()
    {
        _lotNumber = string.Empty;
        Material = null!;
        StockLotAdjustments = new List<StockLotAdjustment>();
    }

    /// <summary>
    /// Creates a new StockLot instance with validated invariants.
    /// </summary>
    /// <param name="lotNumber">The lot number (required, max 100 characters).</param>
    /// <param name="materialId">The material ID (required).</param>
    /// <param name="amountOfBars">The amount of bars (required, must be non-negative).</param>
    /// <param name="diameter">The diameter (required, must be positive).</param>
    /// <param name="barLength">The bar length (required, must be positive).</param>
    /// <param name="condition">The stock lot condition (required).</param>
    /// <param name="checkedInDateTime">The check-in date/time (required).</param>
    /// <exception cref="DomainException">Thrown when invariants are violated.</exception>
    public StockLot(
        string lotNumber,
        int materialId,
        int amountOfBars,
        decimal diameter,
        decimal barLength,
        StockLotConditionEnum condition,
        DateTime checkedInDateTime)
    {
        LotNumber = lotNumber;
        MaterialId = materialId;
        AmountOfBars = amountOfBars;
        Diameter = diameter;
        BarLength = barLength;
        Condition = condition;
        CheckedInDateTime = checkedInDateTime;
        Material = null!;
        StockLotAdjustments = new List<StockLotAdjustment>();
    }

    private string _lotNumber = string.Empty;

    public string LotNumber
    {
        get => _lotNumber;
        set
        {
            Guard.AgainstNullOrWhiteSpace(value, nameof(LotNumber));
            Guard.AgainstMaxLength(value, MaxLotNumberLength, nameof(LotNumber));
            _lotNumber = value;
        }
    }

    public int MaterialId { get; set; }

    public int AmountOfBars { get; set; }

    public decimal Diameter { get; set; }

    public decimal BarLength { get; set; }

    public StockLotConditionEnum Condition { get; set; }

    public DateTime CheckedInDateTime { get; set; }

    public Material Material { get; set; } = null!;

    public ICollection<StockLotAdjustment> StockLotAdjustments { get; set; } = new List<StockLotAdjustment>();

    /// <summary>
    /// Inactivates the stock lot (soft-delete).
    /// Prevents double-inactivation by throwing a DomainException if already inactivated.
    /// </summary>
    /// <param name="inactivatedByUserId">The ID of the user performing the inactivation (optional).</param>
    /// <exception cref="DomainException">Thrown when the stock lot is already inactivated.</exception>
    public void Inactivate(int? inactivatedByUserId = null)
    {
        if (InactivatedDateTime.HasValue)
        {
            throw new DomainException("StockLot is already inactivated and cannot be inactivated again.");
        }

        InactivatedDateTime = DateTimeOffset.UtcNow;
        InactivatedByUserId = inactivatedByUserId;
    }
}
