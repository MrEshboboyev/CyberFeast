namespace BuildingBlocks.Abstractions.Events.Internal;

/// <summary>
/// Defines a structure for domain notification events with a specific domain event type.
/// </summary>
/// <typeparam name="TDomainEventType">The type of the domain event.</typeparam>
public interface IDomainNotificationEvent<TDomainEventType> : IDomainNotificationEvent
    where TDomainEventType : IDomainEvent
{
    /// <summary>
    /// Gets or sets the specific domain event.
    /// </summary>
    TDomainEventType DomainEvent { get; set; }
}

/// <summary>
/// Defines a structure for domain notification events.
/// </summary>
public interface IDomainNotificationEvent : IEvent
{
}