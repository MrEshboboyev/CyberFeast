using BuildingBlocks.Abstractions.Domain;

namespace BuildingBlocks.Core.Domain;

/// <summary>
/// Represents an auditable entity with a generic identifier type.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public class AuditableEntity<TId> : Entity<TId>, IAuditableEntity<TId>
{
    /// <summary>
    /// Gets the date and time when the entity was last modified.
    /// </summary>
    public DateTime? LastModified { get; init; } = null!;

    /// <summary>
    /// Gets the identifier of the user who last modified the entity.
    /// </summary>
    public int? LastModifiedBy { get; init; } = null!;
}

/// <summary>
/// Represents an auditable entity with a specific identity type.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity.</typeparam>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public abstract class AuditableEntity<TIdentity, TId> : AuditableEntity<TIdentity>
    where TIdentity : Identity<TId>
{
}

/// <summary>
/// Represents an auditable entity with a default identity type and identifier type.
/// </summary>
public class AuditableEntity : AuditableEntity<Identity<long>, long>
{
}