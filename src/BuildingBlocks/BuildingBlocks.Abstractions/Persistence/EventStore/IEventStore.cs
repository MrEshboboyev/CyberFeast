using BuildingBlocks.Abstractions.Domain.EventSourcing;

namespace BuildingBlocks.Abstractions.Persistence.EventStore;

/// <summary>
/// Defines methods for interacting with an event store.
/// </summary>
public interface IEventStore
{
    /// <summary>
    /// Checks if a stream exists.
    /// </summary>
    /// <param name="streamId">The ID of the stream to check.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains a boolean indicating if the stream exists.</returns>
    Task<bool> StreamExists(string streamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves events from a stream asynchronously.
    /// </summary>
    /// <param name="streamId">The ID of the stream to retrieve events from.</param>
    /// <param name="fromVersion">The position from which to start reading events.</param>
    /// <param name="maxCount">The maximum number of events to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains an enumerable of stream event envelopes.</returns>
    Task<IEnumerable<IStreamEventEnvelope>> GetStreamEventsAsync(
        string streamId,
        StreamReadPosition? fromVersion = null,
        int maxCount = int.MaxValue,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves events from a stream asynchronously.
    /// </summary>
    /// <param name="streamId">The ID of the stream to retrieve events from.</param>
    /// <param name="fromVersion">The position from which to start reading events.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains an enumerable of stream event envelopes.</returns>
    Task<IEnumerable<IStreamEventEnvelope>> GetStreamEventsAsync(
        string streamId,
        StreamReadPosition? fromVersion = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Appends an event to a stream asynchronously.
    /// </summary>
    /// <param name="streamId">The ID of the stream to append the event to.</param>
    /// <param name="event">The event to append.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains the result of the append operation.</returns>
    Task<AppendResult> AppendEventAsync(
        string streamId,
        IStreamEventEnvelope @event,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Appends an event to a stream with an expected version asynchronously.
    /// </summary>
    /// <param name="streamId">The ID of the stream to append the event to.</param>
    /// <param name="event">The event to append.</param>
    /// <param name="expectedRevision">The expected version of the stream.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains the result of the append operation.</returns>
    Task<AppendResult> AppendEventAsync(
        string streamId,
        IStreamEventEnvelope @event,
        ExpectedStreamVersion expectedRevision,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Appends multiple events to a stream with an expected version asynchronously.
    /// </summary>
    /// <param name="streamId">The ID of the stream to append the events to.</param>
    /// <param name="events">The events to append.</param>
    /// <param name="expectedRevision">The expected version of the stream.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains the result of the append operation.</returns>
    Task<AppendResult> AppendEventsAsync(
        string streamId,
        IReadOnlyCollection<IStreamEventEnvelope> events,
        ExpectedStreamVersion expectedRevision,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aggregates the state of a stream into an aggregate asynchronously.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    /// <param name="streamId">The ID of the stream to aggregate.</param>
    /// <param name="fromVersion">The position from which to start aggregating events.</param>
    /// <param name="defaultAggregateState">The default state of the aggregate.</param>
    /// <param name="fold">The function to apply each event to the aggregate state.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains the aggregated aggregate.</returns>
    Task<TAggregate?> AggregateStreamAsync<TAggregate, TId>(
        string streamId,
        StreamReadPosition fromVersion,
        TAggregate defaultAggregateState,
        Action<object> fold,
        CancellationToken cancellationToken = default)
        where TAggregate : class, IEventSourcedAggregate<TId>, new();

    /// <summary>
    /// Aggregates the state of a stream into an aggregate asynchronously.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    /// <param name="streamId">The ID of the stream to aggregate.</param>
    /// <param name="defaultAggregateState">The default state of the aggregate.</param>
    /// <param name="fold">The function to apply each event to the aggregate state.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains the aggregated aggregate.</returns>
    Task<TAggregate?> AggregateStreamAsync<TAggregate, TId>(
        string streamId,
        TAggregate defaultAggregateState,
        Action<object> fold,
        CancellationToken cancellationToken = default)
        where TAggregate : class, IEventSourcedAggregate<TId>, new();

    /// <summary>
    /// Commits changes to the event store asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);
}