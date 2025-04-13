namespace BuildingBlocks.Abstractions.Domain;

/// <summary>
/// Defines a contract for entities that have an identifier and creation metadata.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IEntity<out TId> : IHaveIdentity<TId>, IHaveCreator;

/// <summary>
/// Defines a contract for entities that have a specific identity type and an identifier.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity.</typeparam>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IEntity<out TIdentity, in TId> : IEntity<TIdentity>
    where TIdentity : IIdentity<TId>;

/// <summary>
/// Defines a contract for entities that use a default EntityId as their identifier.
/// </summary>
public interface IEntity : IEntity<EntityId>;
