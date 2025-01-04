namespace BuildingBlocks.Abstractions.Domain;

/// <summary>
/// Defines a contract for entities that have creation metadata.
/// </summary>
public interface IHaveCreator
{
    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    DateTime Created { get; }

    /// <summary>
    /// Gets the identifier of the user or process that created the entity.
    /// </summary>
    int? CreatedBy { get; }
}