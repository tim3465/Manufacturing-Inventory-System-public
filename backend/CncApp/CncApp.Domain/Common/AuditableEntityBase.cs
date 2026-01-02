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
    /// ID of the Domain User who created the entity. Nullable if created by the system.
    /// This is the Domain UserId (not Identity UserId) - resolved from IdentityUserId at request time.
    /// </summary>
    public int? CreatedByUserId { get; set; }

    /// <summary>
    /// UTC timestamp when the entity was last updated. Null if never updated.
    /// </summary>
    public DateTimeOffset? UpdatedDateTime { get; set; }

    /// <summary>
    /// ID of the Domain User who last updated the entity. Nullable if not updated or updated by the system.
    /// This is the Domain UserId (not Identity UserId) - resolved from IdentityUserId at request time.
    /// </summary>
    public int? UpdatedByUserId { get; set; }

    /// <summary>
    /// UTC timestamp when the entity was inactivated (soft-deleted). Null if the entity is active.
    /// </summary>
    public DateTimeOffset? InactivatedDateTime { get; set; }

    /// <summary>
    /// ID of the Domain User who inactivated the entity. Nullable if not inactivated or inactivated by the system.
    /// This is the Domain UserId (not Identity UserId) - resolved from IdentityUserId at request time.
    /// </summary>
    public int? InactivatedByUserId { get; set; }
}



