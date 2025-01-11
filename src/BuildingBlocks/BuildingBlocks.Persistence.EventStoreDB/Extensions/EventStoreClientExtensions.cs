using BuildingBlocks.Abstractions.Events;
using EventStore.Client;

namespace BuildingBlocks.Persistence.EventStoreDB.Extensions;

/// <summary>
/// Provides extension methods for <see cref="EventStoreClient"/> to interact with EventStoreDB streams.
/// </summary>
public static class EventStoreClientExtensions
{
    /// <summary>
    /// Finds and aggregates events from a stream into an entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="eventStore">The EventStore client.</param>
    /// <param name="getDefault">A function to retrieve the default entity.</param>
    /// <param name="when">A function to apply events to the entity.</param>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The aggregated entity.</returns>
    public static async Task<TEntity> Find<TEntity>(
        this EventStoreClient eventStore,
        Func<TEntity> getDefault,
        Func<TEntity, object, TEntity> when,
        string streamId,
        CancellationToken cancellationToken
    )
    {
        var readResult = eventStore.ReadStreamAsync(
            Direction.Forwards,
            streamId,
            StreamPosition.Start,
            cancellationToken: cancellationToken
        );

        return (await readResult
                .Select(@event => @event.DeserializeData()!)
                .AggregateAsync(getDefault(), when, cancellationToken)
            )!;
    }

    /// <summary>
    /// Appends an event to a stream asynchronously, creating the stream if it doesn't exist.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventStore">The EventStore client.</param>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="event">The event to append.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The next expected stream revision.</returns>
    public static async Task<ulong> Append<TEvent>(
        this EventStoreClient eventStore,
        string streamId,
        TEvent @event,
        CancellationToken cancellationToken
    )
        where TEvent : IDomainEvent
    {
        var result = await eventStore.AppendToStreamAsync(
            streamId,
            StreamState.NoStream,
            [@event.ToJsonEventData()],
            cancellationToken: cancellationToken
        );

        return result.NextExpectedStreamRevision;
    }

    /// <summary>
    /// Appends an event to a stream asynchronously with a specific expected revision.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventStore">The EventStore client.</param>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="event">The event to append.</param>
    /// <param name="expectedRevision">The expected stream revision.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The next expected stream revision.</returns>
    public static async Task<ulong> Append<TEvent>(
        this EventStoreClient eventStore,
        string streamId,
        TEvent @event,
        ulong expectedRevision,
        CancellationToken cancellationToken
    )
        where TEvent : IDomainEvent
    {
        var result = await eventStore.AppendToStreamAsync(
            streamId,
            expectedRevision,
            [@event.ToJsonEventData()],
            cancellationToken: cancellationToken
        );

        return result.NextExpectedStreamRevision;
    }
}