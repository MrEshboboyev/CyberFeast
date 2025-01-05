namespace BuildingBlocks.Abstractions.Events.Internal;

/// <summary>
/// Defines the structure of a domain notification event handler.
/// </summary>
/// <typeparam name="TEvent">The type of the domain notification event.</typeparam>
public interface IDomainNotificationEventHandler<in TEvent> : IEventHandler<TEvent>
    where TEvent : IDomainNotificationEvent
{
}