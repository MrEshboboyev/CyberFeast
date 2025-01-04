namespace BuildingBlocks.Abstractions.Domain;

/// <summary>
/// Defines a contract for entities that have an identifier.
/// </summary>
public interface IHaveIdentity
{
    /// <summary>
    /// Gets the identifier of the entity.
    /// </summary>
    object Id { get; }
}

/// <summary>
/// Defines a contract for entities that have a strongly-typed identifier.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IHaveIdentity<out TId> : IHaveIdentity
{
    /// <summary>
    /// Gets the identifier of the entity as the specified generic type.
    /// </summary>
    new TId Id { get; }

    /// <summary>
    /// Explicit implementation to get the identifier as an object.
    /// </summary>
    object IHaveIdentity.Id => Id!;
}