using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class Material : AuditableEntityBase
{
    private const int MaxHeatNumberLength = 100;
    private const int MaxMaterialNameLength = 100;

    // Private constructor for EF Core
    // Sets backing fields directly to avoid validation during materialization
    private Material()
    {
        _heatNumber = string.Empty;
        _materialName = string.Empty;
        StockLots = new List<StockLot>();
    }

    /// <summary>
    /// Creates a new Material instance with validated invariants.
    /// </summary>
    /// <param name="heatNumber">The material heat number (required, max 100 characters).</param>
    /// <param name="materialName">The material name (required, max 100 characters).</param>
    /// <exception cref="DomainException">Thrown when invariants are violated.</exception>
    public Material(string heatNumber, string materialName)
    {
        HeatNumber = heatNumber;
        MaterialName = materialName;
        StockLots = new List<StockLot>();
    }

    private string _heatNumber = string.Empty;

    public string HeatNumber
    {
        get => _heatNumber;
        set
        {
            Guard.AgainstNullOrWhiteSpace(value, nameof(HeatNumber));
            Guard.AgainstMaxLength(value, MaxHeatNumberLength, nameof(HeatNumber));
            _heatNumber = value;
        }
    }

    private string _materialName = string.Empty;

    public string MaterialName
    {
        get => _materialName;
        set
        {
            Guard.AgainstNullOrWhiteSpace(value, nameof(MaterialName));
            Guard.AgainstMaxLength(value, MaxMaterialNameLength, nameof(MaterialName));
            _materialName = value;
        }
    }

    public ICollection<StockLot> StockLots { get; set; } = new List<StockLot>();

    /// <summary>
    /// Inactivates the material (soft-delete).
    /// Prevents double-inactivation by throwing a DomainException if already inactivated.
    /// </summary>
    /// <param name="inactivatedByUserId">The ID of the user performing the inactivation (optional).</param>
    /// <exception cref="DomainException">Thrown when the material is already inactivated.</exception>
    public void Inactivate(int? inactivatedByUserId = null)
    {
        if (InactivatedDateTime.HasValue)
        {
            throw new DomainException("Material is already inactivated and cannot be inactivated again.");
        }

        InactivatedDateTime = DateTimeOffset.UtcNow;
        InactivatedByUserId = inactivatedByUserId;
    }
}
