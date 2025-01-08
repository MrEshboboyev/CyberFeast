using BuildingBlocks.Abstractions.Domain.EventSourcing;
using BuildingBlocks.Abstractions.Persistence.EventStore;
using BuildingBlocks.Core.Persistence.EventStore.Extensions;

namespace BuildingBlocks.Core.Persistence.EventStore.InMemory;

/// <summary>
/// Represents an in-memory implementation of the event store.
/// </summary>
public class InMemoryEventStore : IEventStore
{
    private readonly Dictionary<string, InMemoryStream> _storage = new();
    private readonly List<StreamEventData> _global = [];

    /// <summary>
    /// Checks if a stream exists in the event store.
    /// </summary>
    /// <param name="streamId">The stream identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if the stream exists; otherwise, <c>false</c>.</returns>
    public Task<bool> StreamExists(
        string streamId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_storage.ContainsKey(streamId));
    }

    /// <summary>
    /// Retrieves events from a stream.
    /// </summary>
    /// <param name="streamId">The stream identifier.</param>
    /// <param name="fromVersion">The version from which to start reading events.</param>
    /// <param name="maxCount">The maximum number of events to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An enumerable of stream event envelopes.</returns>
    public Task<IEnumerable<IStreamEventEnvelope>> GetStreamEventsAsync(
        string streamId,
        StreamReadPosition? fromVersion = null,
        int maxCount = int.MaxValue,
        CancellationToken cancellationToken = default)
    {
        var result = FindStream(streamId)
            .GetEvents(fromVersion ?? StreamReadPosition.Start, maxCount);

        return Task.FromResult(result.Select(x => x.ToStreamEvent()));
    }

    /// <summary>
    /// Retrieves events from a stream.
    /// </summary>
    /// <param name="streamId">The stream identifier.</param>
    /// <param name="fromVersion">The version from which to start reading events.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An enumerable of stream event envelopes.</returns>
    public Task<IEnumerable<IStreamEventEnvelope>> GetStreamEventsAsync(
        string streamId,
        StreamReadPosition? fromVersion = null,
        CancellationToken cancellationToken = default)
    {
        return GetStreamEventsAsync(
            streamId,
            fromVersion,
            int.MaxValue,
            cancellationToken);
    }

    /// <summary>
    /// Appends an event to a stream.
    /// </summary>
    /// <param name="streamId">The stream identifier.</param>
    /// <param name="event">The event to append.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the append operation.</returns>
    public Task<AppendResult> AppendEventAsync(
        string streamId,
        IStreamEventEnvelope @event,
        CancellationToken cancellationToken = default)
    {
        return AppendEventsAsync(
            streamId,
            [@event],
            ExpectedStreamVersion.NoStream,
            cancellationToken);
    }

    /// <summary>
    /// Appends an event to a stream with the specified expected stream version.
    /// </summary>
    /// <param name="streamId">The stream identifier.</param>
    /// <param name="event">The event to append.</param>
    /// <param name="expectedRevision">The expected stream version.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the append operation.</returns>
    public Task<AppendResult> AppendEventAsync(
        string streamId,
        IStreamEventEnvelope @event,
        ExpectedStreamVersion expectedRevision,
        CancellationToken cancellationToken = default)
    {
        return AppendEventsAsync(
            streamId, 
            [@event], 
            expectedRevision, 
            cancellationToken);
    }

    /// <summary>
    /// Appends multiple events to a stream with the specified expected stream version.
    /// </summary>
    /// <param name="streamId">The stream identifier.</param>
    /// <param name="events">The events to append.</param>
    /// <param name="expectedRevision">The expected stream version.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the append operation.</returns>
    public Task<AppendResult> AppendEventsAsync(
        string streamId,
        IReadOnlyCollection<IStreamEventEnvelope> events,
        ExpectedStreamVersion expectedRevision,
        CancellationToken cancellationToken = default)
    {
        if (!_storage.TryGetValue(streamId, out var existing))
        {
            existing = new InMemoryStream(streamId);
            _storage.Add(streamId, existing);
        }

        var inMemoryEvents = events.Select(
            x => x.ToJsonStreamEventData()).ToList();

        existing.AppendEvents(
            expectedRevision, 
            _global.Count - 1, 
            inMemoryEvents);

        _global.AddRange(inMemoryEvents);

        return Task.FromResult(
            new AppendResult(_global.Count - 1, existing.Version));
    }

    /// <summary>
    /// Aggregates events from a stream to recreate an aggregate's state.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TId">The type of the aggregate ID.</typeparam>
    /// <param name="streamId">The stream identifier.</param>
    /// <param name="fromVersion">The version from which to start reading events.</param>
    /// <param name="defaultAggregateState">The default state of the aggregate.</param>
    /// <param name="fold">The fold function to apply events to the aggregate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The aggregated aggregate state.</returns>
    public Task<TAggregate?> AggregateStreamAsync<TAggregate, TId>(
        string streamId,
        StreamReadPosition fromVersion,
        TAggregate defaultAggregateState,
        Action<object> fold,
        CancellationToken cancellationToken = default)
        where TAggregate : class, IEventSourcedAggregate<TId>, new()
    {
        var streamEvents = FindStream(streamId)
            .GetEvents(fromVersion, int.MaxValue)
            .Select(x => x.DeserializeData());

        var result = streamEvents.Aggregate(
            defaultAggregateState,
            (agg, @event) =>
            {
                fold(@event);
                return agg;
            }
        );

        return Task.FromResult(result)!;
    }

    /// <summary>
    /// Aggregates events from a stream to recreate an aggregate's state.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TId">The type of the aggregate ID.</typeparam>
    /// <param name="streamId">The stream identifier.</param>
    /// <param name="defaultAggregateState">The default state of the aggregate.</param>
    /// <param name="fold">The fold function to apply events to the aggregate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The aggregated aggregate state.</returns>
    public Task<TAggregate?> AggregateStreamAsync<TAggregate, TId>(
        string streamId,
        TAggregate defaultAggregateState,
        Action<object> fold,
        CancellationToken cancellationToken = default)
        where TAggregate : class, IEventSourcedAggregate<TId>, new()
    {
        return AggregateStreamAsync<TAggregate, TId>(
            streamId, 
            StreamReadPosition.Start, 
            defaultAggregateState, 
            fold,
            cancellationToken);
    }

    /// <summary>
    /// Commits the changes to the event store.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Finds a stream in the event store.
    /// </summary>
    /// <param name="stream">The stream identifier.</param>
    /// <returns>The in-memory stream.</returns>
    /// <exception cref="System.Exception">Thrown when the stream is not found.</exception>
    private InMemoryStream FindStream(string stream)
    {
        if (!_storage.TryGetValue(stream, out var existing))
            throw new System.Exception($"Stream with name: {stream} not found.");

        return existing;
    }
}