using BuildingBlocks.Abstractions.Domain;

namespace BuildingBlocks.Core.Domain;

/// <summary>
/// Represents a base entity with a generic identifier type.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public abstract class Entity<TId> : IEntity<TId>
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    public TId Id { get; init; } = default!;

    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    public DateTime Created { get; init; } = default!;

    /// <summary>
    /// Gets the identifier of the user who created the entity.
    /// </summary>
    public int? CreatedBy { get; init; } = null!;
}

/// <summary>
/// Represents a base entity with a specific identity type.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity.</typeparam>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public abstract class Entity<TIdentity, TId> : Entity<TIdentity>
    where TIdentity : Identity<TId>
{
}

/// <summary>
/// Represents a base entity with a default identity type and identifier type.
/// </summary>
public abstract class Entity : Entity<EntityId, long>, IEntity
{
}