using System.Collections.Immutable;
using BuildingBlocks.Abstractions.Domain.EventSourcing;
using BuildingBlocks.Abstractions.Persistence.EventStore;
using BuildingBlocks.Persistence.EventStoreDB.Extensions;
using EventStore.Client;

namespace BuildingBlocks.Persistence.EventStoreDB;

/// <summary>
/// Provides an implementation of the <see cref="IEventStore"/> interface for interacting with EventStoreDB.
/// </summary>
public class EventStoreDbEventStore(EventStoreClient grpcClient) : IEventStore
{
    /// <summary>
    /// Checks if a stream with the specified ID exists in EventStoreDB.
    /// </summary>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the stream exists, otherwise false.</returns>
    public async Task<bool> StreamExists(
        string streamId,
        CancellationToken cancellationToken = default)
    {
        var read = grpcClient.ReadStreamAsync(
            Direction.Forwards,
            streamId,
            StreamPosition.Start,
            1,
            cancellationToken: cancellationToken
        );

        var state = await read.ReadState;
        return state == ReadState.Ok;
    }

    /// <summary>
    /// Retrieves events from a stream asynchronously.
    /// </summary>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="fromVersion">The starting version to read from.</param>
    /// <param name="maxCount">The maximum number of events to read.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of stream event envelopes.</returns>
    public async Task<IEnumerable<IStreamEventEnvelope>> GetStreamEventsAsync(
        string streamId,
        StreamReadPosition? fromVersion = null,
        int maxCount = int.MaxValue,
        CancellationToken cancellationToken = default)
    {
        var readResult = grpcClient.ReadStreamAsync(
            Direction.Forwards,
            streamId,
            fromVersion?.AsStreamPosition() ?? StreamPosition.Start,
            maxCount,
            cancellationToken: cancellationToken
        );

        var resolvedEvents = await readResult.ToListAsync(cancellationToken);

        return resolvedEvents.ToStreamEvents();
    }

    /// <summary>
    /// Retrieves all events from a stream asynchronously.
    /// </summary>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="fromVersion">The starting version to read from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of stream event envelopes.</returns>
    public Task<IEnumerable<IStreamEventEnvelope>> GetStreamEventsAsync(
        string streamId,
        StreamReadPosition? fromVersion = null,
        CancellationToken cancellationToken = default)
    {
        return GetStreamEventsAsync(streamId, fromVersion, int.MaxValue, cancellationToken);
    }

    /// <summary>
    /// Appends a single event to a stream asynchronously.
    /// </summary>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="event">The event to append.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The append result with log position and next expected stream revision.</returns>
    public Task<AppendResult> AppendEventAsync(
        string streamId,
        IStreamEventEnvelope @event,
        CancellationToken cancellationToken = default)
    {
        return AppendEventsAsync(
            streamId,
            new List<IStreamEventEnvelope> { @event }.ToImmutableList(),
            ExpectedStreamVersion.NoStream,
            cancellationToken
        );
    }

    /// <summary>
    /// Appends a single event to a stream with a specific expected version asynchronously.
    /// </summary>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="event">The event to append.</param>
    /// <param name="expectedRevision">The expected stream version.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The append result with log position and next expected stream revision.</returns>
    public Task<AppendResult> AppendEventAsync(
        string streamId,
        IStreamEventEnvelope @event,
        ExpectedStreamVersion expectedRevision,
        CancellationToken cancellationToken = default)
    {
        return AppendEventsAsync(
            streamId,
            new List<IStreamEventEnvelope> { @event }.ToImmutableList(),
            expectedRevision,
            cancellationToken
        );
    }

    /// <summary>
    /// Appends multiple events to a stream asynchronously.
    /// </summary>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="events">The events to append.</param>
    /// <param name="expectedRevision">The expected stream version.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The append result with log position and next expected stream revision.</returns>
    public async Task<AppendResult> AppendEventsAsync(
        string streamId,
        IReadOnlyCollection<IStreamEventEnvelope> events,
        ExpectedStreamVersion expectedRevision,
        CancellationToken cancellationToken = default)
    {
        var eventsData = events.Select(x => x.ToJsonEventData());

        if (expectedRevision == ExpectedStreamVersion.NoStream)
        {
            var result = await grpcClient.AppendToStreamAsync(
                streamId,
                StreamState.NoStream,
                eventsData,
                cancellationToken: cancellationToken
            );

            return new AppendResult(
                (long)result.LogPosition.CommitPosition,
                result.NextExpectedStreamRevision.ToInt64()
            );
        }

        if (expectedRevision == ExpectedStreamVersion.Any)
        {
            var result = await grpcClient.AppendToStreamAsync(
                streamId,
                StreamState.Any,
                eventsData,
                cancellationToken: cancellationToken
            );

            return new AppendResult(
                (long)result.LogPosition.CommitPosition,
                result.NextExpectedStreamRevision.ToInt64()
            );
        }
        else
        {
            var result = await grpcClient.AppendToStreamAsync(
                streamId,
                expectedRevision.AsStreamRevision(),
                eventsData,
                cancellationToken: cancellationToken
            );

            return new AppendResult(
                (long)result.LogPosition.CommitPosition,
                result.NextExpectedStreamRevision.ToInt64()
            );
        }
    }

    /// <summary>
    /// Aggregates events from a stream into an event-sourced aggregate asynchronously.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="fromVersion">The starting version to read from.</param>
    /// <param name="defaultAggregateState">The default state of the aggregate.</param>
    /// <param name="fold">An action to apply events to the aggregate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The aggregated event-sourced aggregate, or null if the stream is not found.</returns>
    public async Task<TAggregate?> AggregateStreamAsync<TAggregate, TId>(
        string streamId,
        StreamReadPosition fromVersion,
        TAggregate defaultAggregateState,
        Action<object> fold,
        CancellationToken cancellationToken = default
    )
        where TAggregate : class, IEventSourcedAggregate<TId>, new()
    {
        var readResult = grpcClient.ReadStreamAsync(
            Direction.Forwards,
            streamId,
            fromVersion.AsStreamPosition(),
            cancellationToken: cancellationToken
        );

        if (await readResult.ReadState.ConfigureAwait(false) == ReadState.StreamNotFound)
            return null;

        return await readResult
            .Select(@event => @event.DeserializeData()!)
            .AggregateAsync(
                defaultAggregateState,
                (agg, @event) =>
                {
                    fold(@event);
                    return agg;
                },
                cancellationToken
            );
    }

    /// <summary>
    /// Aggregates events from a stream into an event-sourced aggregate asynchronously.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="defaultAggregateState">The default state of the aggregate.</param>
    /// <param name="fold">An action to apply events to the aggregate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The aggregated event-sourced aggregate, or null if the stream is not found.</returns>
    public Task<TAggregate?> AggregateStreamAsync<TAggregate, TId>(
        string streamId,
        TAggregate defaultAggregateState,
        Action<object> fold,
        CancellationToken cancellationToken = default
    )
        where TAggregate : class, IEventSourcedAggregate<TId>, new()
    {
        return AggregateStreamAsync<TAggregate, TId>(
            streamId,
            StreamReadPosition.Start,
            defaultAggregateState,
            fold,
            cancellationToken
        );
    }

    /// <summary>
    /// Commits the changes asynchronously. Since there are no transactional changes, this method completes immediately.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public Task CommitAsync(CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}