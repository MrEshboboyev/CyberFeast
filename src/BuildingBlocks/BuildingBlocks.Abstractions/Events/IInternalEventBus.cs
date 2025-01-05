using BuildingBlocks.Abstractions.Persistence.EventStore;

namespace BuildingBlocks.Abstractions.Events;

/// <summary>
/// Defines methods for publishing in-memory events.
/// </summary>
public interface IInternalEventBus
{
    /// <summary>
    /// Publishes a single in-memory event.
    /// </summary>
    /// <param name="eventData">The event data to publish.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Publish(IEvent eventData, CancellationToken ct);

    /// <summary>
    /// Publishes multiple in-memory events.
    /// </summary>
    /// <param name="eventsData">The events data to publish.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Publish(IEnumerable<IEvent> eventsData, CancellationToken ct);

    /// <summary>
    /// Publishes an in-memory event based on a consumed event from a messaging system.
    /// </summary>
    /// <param name="eventEnvelope">The event envelope containing the event and its metadata.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Publish(IEventEnvelope eventEnvelope, CancellationToken ct);

    /// <summary>
    /// Publishes a strongly-typed in-memory event based on a consumed event from a messaging system.
    /// </summary>
    /// <typeparam name="T">The type of the event.</typeparam>
    /// <param name="eventEnvelope">The event envelope containing the event and its metadata.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Publish<T>(IEventEnvelope<T> eventEnvelope, CancellationToken ct)
        where T : class;

    /// <summary>
    /// Publishes an in-memory event based on consumed events from an event store.
    /// </summary>
    /// <param name="streamEventEnvelope">The stream event envelope containing the event and its metadata.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Publish(IStreamEventEnvelope streamEventEnvelope, CancellationToken ct);

    /// <summary>
    /// Publishes a strongly-typed in-memory event based on consumed events from an event store.
    /// </summary>
    /// <typeparam name="T">The type of the event.</typeparam>
    /// <param name="streamEventEnvelope">The stream event envelope containing the event and its metadata.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Publish<T>(IStreamEventEnvelope<T> streamEventEnvelope, CancellationToken ct)
        where T : IDomainEvent;
}