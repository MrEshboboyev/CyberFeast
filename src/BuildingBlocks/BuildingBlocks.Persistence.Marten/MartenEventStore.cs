using System.Collections.Immutable;
using BuildingBlocks.Abstractions.Domain.EventSourcing;
using BuildingBlocks.Abstractions.Persistence.EventStore;
using Marten;

namespace BuildingBlocks.Persistence.Marten;

/// <summary>
/// Provides an implementation of the <see cref="IEventStore"/> interface using Marten for event sourcing.
/// </summary>
public class MartenEventStore(IDocumentSession documentSession) : IEventStore
{
    /// <summary>
    /// Checks if a stream with the specified ID exists in Marten.
    /// </summary>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the stream exists, otherwise false.</returns>
    public async Task<bool> StreamExists(string streamId, CancellationToken cancellationToken = default)
    {
        var state = await documentSession.Events.FetchStreamStateAsync(streamId, cancellationToken);
        return await Task.FromResult(state != null);
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
        CancellationToken cancellationToken = default
    )
    {
        var events = await Filter(streamId, fromVersion?.Value, null).ToListAsync(cancellationToken);
        var streamEvents = events.Select(ev => ev.Data).OfType<IStreamEventEnvelope>().ToImmutableList();
        return streamEvents;
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
        CancellationToken cancellationToken = default
    )
    {
        return GetStreamEventsAsync(streamId, fromVersion, int.MaxValue, cancellationToken);
    }

    /// <summary>
    /// Appends a single event to a stream asynchronously.
    /// </summary>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="event">The event to append.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The append result with the next expected stream revision.</returns>
    public async Task<AppendResult> AppendEventAsync(
        string streamId,
        IStreamEventEnvelope @event,
        CancellationToken cancellationToken = default
    )
    {
        var result = documentSession.Events.Append(streamId, @event);
        var nextVersionsList = await documentSession.Events.FetchStreamAsync(streamId, token: cancellationToken);
        return await Task.FromResult(new AppendResult(-1, nextVersionsList.Count));
    }

    /// <summary>
    /// Appends a single event to a stream with a specific expected version asynchronously.
    /// </summary>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="event">The event to append.</param>
    /// <param name="expectedRevision">The expected stream version.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The append result with the next expected stream revision.</returns>
    public Task<AppendResult> AppendEventAsync(
        string streamId,
        IStreamEventEnvelope @event,
        ExpectedStreamVersion expectedRevision,
        CancellationToken cancellationToken = default
    )
    {
        return AppendEventsAsync(streamId, new List<IStreamEventEnvelope> { @event }, expectedRevision,
            cancellationToken);
    }

    /// <summary>
    /// Appends multiple events to a stream asynchronously.
    /// </summary>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="events">The events to append.</param>
    /// <param name="expectedRevision">The expected stream version.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The append result with the next expected stream revision.</returns>
    public async Task<AppendResult> AppendEventsAsync(
        string streamId,
        IReadOnlyCollection<IStreamEventEnvelope> events,
        ExpectedStreamVersion expectedRevision,
        CancellationToken cancellationToken = default
    )
    {
        var result = documentSession.Events.Append(
            streamId,
            expectedVersion: expectedRevision.Value,
            events: events.Cast<object>().ToArray()
        );

        var nextVersion = expectedRevision.Value + events.Count;
        return await Task.FromResult(new AppendResult(-1, nextVersion));
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
        var aggregate = await documentSession.Events.AggregateStreamAsync<TAggregate>(
            streamId,
            version: fromVersion.Value,
            token: cancellationToken
        );

        if (aggregate == null) return null;

        fold(aggregate);
        return aggregate;
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
    public async Task<TAggregate?> AggregateStreamAsync<TAggregate, TId>(
        string streamId,
        TAggregate defaultAggregateState,
        Action<object> fold,
        CancellationToken cancellationToken = default
    )
        where TAggregate : class, IEventSourcedAggregate<TId>, new()
    {
        var aggregate = await documentSession.Events.AggregateStreamAsync<TAggregate>(
            streamId,
            version: StreamReadPosition.Start.Value,
            token: cancellationToken
        );

        if (aggregate == null) return null;

        fold(aggregate);
        return aggregate;
    }

    /// <summary>
    /// Commits the changes asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return documentSession.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Filters events based on stream ID, version, and timestamp.
    /// </summary>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="version">The starting version to read</param>
    /// <param name="timestamp"></param>
    private IQueryable<global::Marten.Events.IEvent> Filter(
        string streamId,
        long? version,
        DateTime? timestamp)
    {
        var query = documentSession.Events.QueryAllRawEvents().AsQueryable();

        query = query.Where(ev => ev.StreamKey == streamId);

        if (version.HasValue)
            query = query.Where(ev => ev.Version >= version);

        if (timestamp.HasValue)
            query = query.Where(ev => ev.Timestamp >= timestamp);

        return query;
    }
}