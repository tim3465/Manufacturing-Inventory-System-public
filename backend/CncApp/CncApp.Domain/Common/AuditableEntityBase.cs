namespace CncApp.Domain.Common;

/// <summary>
/// Base class for entities that require auditing and soft-delete functionality.
/// Inherits from EntityBase to provide identity, and adds creation, update, and inactivation tracking.
/// </summary>
public abstract class AuditableEntityBase : EntityBase
{
    /// <summary>
    /// UTC timestamp when the entity was created.
    /// </summary>
    public DateTimeOffset CreatedDateTime { get; set; }

    /// <summary>
    /// ID of the user who created the entity. Nullable if created by the system.
    /// </summary>
    public int? CreatedByUserId { get; set; }

    /// <summary>
    /// UTC timestamp when the entity was last updated. Null if never updated.
    /// </summary>
    public DateTimeOffset? UpdatedDateTime { get; set; }

    /// <summary>
    /// ID of the user who last updated the entity. Nullable if not updated or updated by the system.
    /// </summary>
    public int? UpdatedByUserId { get; set; }

    /// <summary>
    /// UTC timestamp when the entity was inactivated (soft-deleted). Null if the entity is active.
    /// </summary>
    public DateTimeOffset? InactivatedDateTime { get; set; }

    /// <summary>
    /// ID of the user who inactivated the entity. Nullable if not inactivated or inactivated by the system.
    /// </summary>
    public int? InactivatedByUserId { get; set; }
}


