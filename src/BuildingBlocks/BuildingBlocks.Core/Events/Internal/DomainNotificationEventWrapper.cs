using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Core.Events.Internal;

/// <summary>
/// Wraps a domain event in a notification event.
/// </summary>
/// <typeparam name="TDomainEventType">The type of the domain event.</typeparam>
/// <param name="DomainEvent">The domain event being wrapped.</param>
public record DomainNotificationEventWrapper<TDomainEventType>(
    TDomainEventType DomainEvent) : DomainNotificationEvent
    where TDomainEventType : IDomainEvent;