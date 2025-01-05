namespace BuildingBlocks.Abstractions.Domain;

/// <summary>
/// Defines an aggregate in domain-driven design (DDD) with a specific identifier type.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IAggregate<out TId> : IEntity<TId>, IHaveAggregate
{
}

/// <summary>
/// Defines an aggregate with a specific identity and identifier type.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity.</typeparam>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IAggregate<out TIdentity, TId> : IAggregate<TIdentity>
    where TIdentity : Identity<TId>
{
}

/// <summary>
/// Defines an aggregate with AggregateId as the identity type and long as the identifier type.
/// </summary>
public interface IAggregate : IAggregate<AggregateId, long>
{
}