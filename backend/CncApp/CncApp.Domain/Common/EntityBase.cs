namespace CncApp.Domain.Common;

/// <summary>
/// Base class for all domain entities providing a consistent identity pattern.
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Primary key identifier for the entity.
    /// </summary>
    public int Id { get; set; }

}

