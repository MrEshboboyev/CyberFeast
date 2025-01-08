using BuildingBlocks.Abstractions.Events.Internal;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Core.Extensions;

namespace BuildingBlocks.Core.Events;

/// <summary>
/// Provides functionality to publish domain notification events using the message persistence service.
/// </summary>
public class DomainNotificationEventPublisher(IMessagePersistenceService messagePersistenceService)
    : IDomainNotificationEventPublisher
{
    /// <summary>
    /// Publishes a domain notification event asynchronously.
    /// </summary>
    /// <param name="domainNotificationEvent">The domain notification event to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task PublishAsync(
        IDomainNotificationEvent domainNotificationEvent,
        CancellationToken cancellationToken = default)
    {
        domainNotificationEvent.NotBeNull();
        return messagePersistenceService.AddNotificationAsync(domainNotificationEvent, cancellationToken);
    }

    /// <summary>
    /// Publishes an array of domain notification events asynchronously.
    /// </summary>
    /// <param name="domainNotificationEvents">The array of domain notification events to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PublishAsync(
        IDomainNotificationEvent[] domainNotificationEvents,
        CancellationToken cancellationToken = default)
    {
        domainNotificationEvents.NotBeNull();

        foreach (var domainNotificationEvent in domainNotificationEvents)
        {
            await messagePersistenceService.AddNotificationAsync(domainNotificationEvent, cancellationToken);
        }
    }
}