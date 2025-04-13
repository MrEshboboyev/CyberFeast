namespace BuildingBlocks.Abstractions.Domain;

/// <summary>
/// Defines a contract for auditable entities with an identifier and audit metadata.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IAuditableEntity<out TId> : IEntity<TId>, IHaveAudit;

/// <summary>
/// Defines a contract for auditable entities with a specific identity type and an identifier.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity.</typeparam>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IAuditableEntity<out TIdentity, TId> : IAuditableEntity<TIdentity>
    where TIdentity : Identity<TId>;

/// <summary>
/// Defines a contract for auditable entities that use a default EntityId as their identifier.
/// </summary>
public interface IAuditableEntity : IAuditableEntity<Identity<long>, long>;
