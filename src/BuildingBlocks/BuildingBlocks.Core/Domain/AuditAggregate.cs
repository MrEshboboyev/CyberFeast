using BuildingBlocks.Abstractions.Domain;

namespace BuildingBlocks.Core.Domain;

/// <summary>
/// Represents an auditable aggregate root.
/// </summary>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
public abstract class AuditAggregate<TId> : Aggregate<TId>, IAuditableEntity<TId>
{
    public DateTime? LastModified { get; protected set; } = null!;
    public int? LastModifiedBy { get; protected set; } = null!;
}

/// <summary>
/// Represents an auditable aggregate root with a specified identity type and identifier type.
/// </summary>
/// <typeparam name="TIdentity">The type of the aggregate identity.</typeparam>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
public abstract class AuditAggregate<TIdentity, TId> : AuditAggregate<TIdentity>
    where TIdentity : Identity<TId>;

/// <summary>
/// Represents an auditable aggregate root with a default identity type and identifier type.
/// </summary>
public abstract class AuditAggregate : AuditAggregate<Identity<long>, long>;