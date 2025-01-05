using BuildingBlocks.Abstractions.Persistence.EventStore;

namespace BuildingBlocks.Abstractions.Domain.EventSourcing;

/// <summary>
/// Defines an event-sourced aggregate in domain-driven design (DDD) with a specific identifier type.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IEventSourcedAggregate<out TId> : IEntity<TId>, IHaveEventSourcingAggregate
{
}

/// <summary>
/// Defines an event-sourced aggregate with a specific identity and identifier type.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity.</typeparam>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IEventSourcedAggregate<out TIdentity, TId> : IEventSourcedAggregate<TIdentity>
    where TIdentity : Identity<TId>
{
}

/// <summary>
/// Defines an event-sourced aggregate with AggregateId as the identity type and long as the identifier type.
/// </summary>
public interface IEventSourcedAggregate : IEventSourcedAggregate<AggregateId, long>
{
}