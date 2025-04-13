namespace BuildingBlocks.Abstractions.Domain;

/// <summary>
/// Defines a contract for entities that have audit metadata,
/// including creation and modification details.
/// </summary>
public interface IHaveAudit : IHaveCreator
{
    /// <summary>
    /// Gets the date and time when the entity was last modified.
    /// </summary>
    DateTime? LastModified { get; }

    /// <summary>
    /// Gets the identifier of the user or process that last modified the entity.
    /// </summary>
    int? LastModifiedBy { get; }
}
