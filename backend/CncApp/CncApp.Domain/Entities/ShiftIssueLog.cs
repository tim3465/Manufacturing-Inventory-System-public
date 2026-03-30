using CncApp.Domain.Common;
using CncApp.Domain.Enums;

namespace CncApp.Domain.Entities;

public class ShiftIssueLog : AuditableEntityBase
{
    public const int MaxDescriptionLength = 2000;

    // Private constructor for EF Core
    // Sets backing fields directly to avoid validation during materialization
    private ShiftIssueLog()
    {
        _description = string.Empty;
        Shift = null!;
    }

    /// <summary>
    /// Creates a new ShiftIssueLog instance with validated invariants.
    /// </summary>
    /// <param name="shiftId">The shift ID (required, must be greater than 0).</param>
    /// <param name="issueType">The issue type (required).</param>
    /// <param name="scrapQuantity">The scrap quantity (required, must be >= 0).</param>
    /// <param name="description">The description (required, max 2000 characters).</param>
    /// <param name="downtime">The downtime (optional).</param>
    /// <exception cref="DomainException">Thrown when invariants are violated.</exception>
    public ShiftIssueLog(int shiftId, IssueTypeEnum issueType, int scrapQuantity, string description, TimeSpan? downtime = null)
    {
        ShiftId = shiftId;
        IssueType = issueType;
        ScrapQuantity = scrapQuantity;
        Description = description;
        Downtime = downtime;

        // Cross-field validation: at least one of ScrapQuantity or Downtime must have a meaningful value
        if (scrapQuantity == 0 && (downtime == null || downtime == TimeSpan.Zero))
        {
            throw new DomainException("At least one of ScrapQuantity or Downtime must have a non-zero value.");
        }

        Shift = null!;
    }

    private int _shiftId;

    public int ShiftId
    {
        get => _shiftId;
        set
        {
            if (value <= 0)
            {
                throw new DomainException("ShiftId must be greater than 0.");
            }
            _shiftId = value;
        }
    }

    public IssueTypeEnum IssueType { get; set; }

    private int _scrapQuantity;

    public int ScrapQuantity
    {
        get => _scrapQuantity;
        set
        {
            if (value < 0)
            {
                throw new DomainException("ScrapQuantity must be non-negative.");
            }
            _scrapQuantity = value;
        }
    }

    public TimeSpan? Downtime { get; set; }

    private string _description = string.Empty;

    public string Description
    {
        get => _description;
        set
        {
            Guard.AgainstNullOrWhiteSpace(value, nameof(Description));
            Guard.AgainstMaxLength(value, MaxDescriptionLength, nameof(Description));
            _description = value;
        }
    }

    public Shift Shift { get; set; } = null!;
}
