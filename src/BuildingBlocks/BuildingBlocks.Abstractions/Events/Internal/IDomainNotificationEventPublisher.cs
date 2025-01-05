namespace BuildingBlocks.Abstractions.Events.Internal;

/// <summary>
/// Defines methods for publishing domain notification events.
/// </summary>
public interface IDomainNotificationEventPublisher
{
    /// <summary>
    /// Asynchronously publishes a single domain notification event.
    /// </summary>
    /// <param name="domainNotificationEvent">The domain notification event to publish.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync(
        IDomainNotificationEvent domainNotificationEvent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously publishes multiple domain notification events.
    /// </summary>
    /// <param name="domainNotificationEvents">The domain notification events to publish.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync(
        IDomainNotificationEvent[] domainNotificationEvents,
        CancellationToken cancellationToken = default);
}