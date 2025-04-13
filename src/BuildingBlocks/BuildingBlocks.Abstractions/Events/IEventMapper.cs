using BuildingBlocks.Abstractions.Events.Internal;
using BuildingBlocks.Abstractions.Messages;

namespace BuildingBlocks.Abstractions.Events;

/// <summary>
/// Defines a mapper for events, extending the domain notification event mapper and the integration event mapper.
/// </summary>
public interface IEventMapper : IIDomainNotificationEventMapper, IIntegrationEventMapper
{
    // The interface can be extended to include additional event mapping functionality in the future.
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

/// <summary>
/// Defines methods for mapping domain events to integration events.
/// </summary>
public interface IIntegrationEventMapper
{
    /// <summary>
    /// Maps a list of domain events to a list of integration events.
    /// </summary>
    /// <param name="domainEvents">The list of domain events to map.</param>
    /// <returns>A read-only list of mapped integration events.</returns>
    IReadOnlyList<IIntegrationEvent?>? MapToIntegrationEvents(IReadOnlyList<IDomainEvent> domainEvents);

    /// <summary>
    /// Maps a single domain event to an integration event.
    /// </summary>
    /// <param name="domainEvent">The domain event to map.</param>
    /// <returns>The mapped integration event.</returns>
    IIntegrationEvent? MapToIntegrationEvent(IDomainEvent domainEvent);
}