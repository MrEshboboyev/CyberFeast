namespace BuildingBlocks.Abstractions.Events;

/// <summary>
/// Defines methods for publishing domain events.
/// </summary>
public interface IDomainEventPublisher
{
    /// <summary>
    /// Asynchronously publishes a single domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to publish.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously publishes multiple domain events.
    /// </summary>
    /// <param name="domainEvents">The domain events to publish.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync(
        IDomainEvent[] domainEvents,
        CancellationToken cancellationToken = default);
}