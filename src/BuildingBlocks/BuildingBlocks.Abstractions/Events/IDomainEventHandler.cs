namespace BuildingBlocks.Abstractions.Events;

/// <summary>
/// Defines the structure of a domain event handler.
/// </summary>
/// <typeparam name="TEvent">The type of the domain event.</typeparam>
public interface IDomainEventHandler<in TEvent> : IEventHandler<TEvent>
    where TEvent : IDomainEvent
{
}