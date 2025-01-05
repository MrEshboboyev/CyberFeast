using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Abstractions.Persistence;

/// <summary>
/// Defines a generic repository for managing events in an asynchronous manner.
/// </summary>
/// <typeparam name="TContext">The type of the context.</typeparam>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public interface IEventRepository<TContext, TEvent>
    where TEvent : IEvent
{
    /// <summary>
    /// Asynchronously inserts a single event.
    /// </summary>
    /// <param name="event">The event to insert.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InsertEvent(TEvent @event, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously inserts multiple events.
    /// </summary>
    /// <param name="events">The events to insert.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InsertRangeEvent(IList<TEvent> events, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously updates a single event.
    /// </summary>
    /// <param name="event">The event to update.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateEvent(TEvent @event, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously updates multiple events.
    /// </summary>
    /// <param name="events">The events to update.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateRangeEvent(IList<TEvent> events, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously deletes a single event.
    /// </summary>
    /// <param name="event">The event to delete.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteEvent(TEvent @event, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously deletes multiple events.
    /// </summary>
    /// <param name="events">The events to delete.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteRangeEvent(IList<TEvent> events, CancellationToken cancellationToken);
}