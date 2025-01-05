using BuildingBlocks.Abstractions.Events.Internal;

namespace BuildingBlocks.Abstractions.Events;

/// <summary>
/// Defines a mapper for events, extending the domain notification event mapper.
/// </summary>
public interface IEventMapper : IIDomainNotificationEventMapper
{
    // The interface can be extended to include integration event mapping in the future.
    // IIntegrationEventMapper functionality will be added here.
}

/// <summary>
/// Defines methods for mapping domain events to domain notification events.
/// </summary>
public interface IIDomainNotificationEventMapper
{
    /// <summary>
    /// Maps a list of domain events to a list of domain notification events.
    /// </summary>
    /// <param name="domainEvents">The list of domain events to map.</param>
    /// <returns>A read-only list of mapped domain notification events.</returns>
    IReadOnlyList<IDomainNotificationEvent?>? MapToDomainNotificationEvents(IReadOnlyList<IDomainEvent> domainEvents);

    /// <summary>
    /// Maps a single domain event to a domain notification event.
    /// </summary>
    /// <param name="domainEvent">The domain event to map.</param>
    /// <returns>The mapped domain notification event.</returns>
    IDomainNotificationEvent? MapToDomainNotificationEvent(IDomainEvent domainEvent);
}

// with Integration event (coming soon) 