using System.Collections.Immutable;
using BuildingBlocks.Abstractions.Domain.EventSourcing;
using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Persistence.EventStore;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Persistence.EventStore.Extensions;

namespace BuildingBlocks.Core.Persistence.EventStore;

/// <summary>
/// Provides a base implementation for an aggregate store that manages aggregates using an event store.
/// </summary>
public abstract class AggregateStore(
    IEventStore eventStore,
    IAggregatesDomainEventsRequestStorage aggregatesDomainEventsStorage) : IAggregateStore
{
    /// <summary>
    /// Loads the aggregate from the store using an aggregate ID.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TId">The type of the aggregate ID.</typeparam>
    /// <param name="aggregateId">The ID of the aggregate.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task with the aggregate as the result.</returns>
    public async Task<TAggregate?> GetAsync<TAggregate, TId>(
        TId aggregateId,
        CancellationToken cancellationToken = default)
        where TAggregate : class, IEventSourcedAggregate<TId>, new()
    {
        aggregateId.NotBeNull();

        var streamName = StreamName.For<TAggregate, TId>(aggregateId);

        var defaultAggregateState = AggregateFactory<TAggregate>.CreateAggregate();

        var result = await eventStore.AggregateStreamAsync<TAggregate, TId>(
            streamName,
            StreamReadPosition.Start,
            defaultAggregateState,
            defaultAggregateState.Fold,
            cancellationToken
        );

        return result;
    }

    /// <summary>
    /// Stores an aggregate state to the store using events.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TId">The type of the aggregate ID.</typeparam>
    /// <param name="aggregate">The aggregate object to be saved.</param>
    /// <param name="expectedVersion">The expected version saved from earlier. -1 if new.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task of the operation result.</returns>
    public async Task<AppendResult> StoreAsync<TAggregate, TId>(
        TAggregate aggregate,
        ExpectedStreamVersion? expectedVersion = null,
        CancellationToken cancellationToken = default)
        where TAggregate : class, IEventSourcedAggregate<TId>, new()
    {
        aggregate.NotBeNull();

        var streamName = StreamName.For<TAggregate, TId>(aggregate.Id);

        ExpectedStreamVersion version = expectedVersion
                                        ?? new ExpectedStreamVersion(aggregate.OriginalVersion);

        var events = aggregate.GetUncommittedDomainEvents();

        // Update events with aggregate ID and event versions
        foreach (var item in events.Select((value, i) => new { index = i, value }))
        {
            item.value.WithAggregate(
                aggregate.Id!,
                aggregate.CurrentVersion + (item.index + 1));
        }

        var streamEvents = events
            .Select(x => x.ToStreamEvent(new StreamEventMetadata(
                x.EventId.ToString(),
                (ulong)x.AggregateSequenceNumber,
                null))
            )
            .ToImmutableList();

        var result = await eventStore.AppendEventsAsync(
            streamName,
            streamEvents,
            version,
            cancellationToken);

        aggregatesDomainEventsStorage.AddEvents(events);

        aggregate.MarkUncommittedDomainEventAsCommitted();

        await eventStore.CommitAsync(cancellationToken);

        return result;
    }

    /// <summary>
    /// Stores an aggregate state to the store using events.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TId">The type of the aggregate ID.</typeparam>
    /// <param name="aggregate">The aggregate object to be saved.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task of the operation result.</returns>
    public Task<AppendResult> StoreAsync<TAggregate, TId>(
        TAggregate aggregate,
        CancellationToken cancellationToken = default)
        where TAggregate : class, IEventSourcedAggregate<TId>, new()
    {
        return StoreAsync<TAggregate, TId>(
            aggregate,
            new ExpectedStreamVersion(aggregate.OriginalVersion),
            cancellationToken);
    }

    /// <summary>
    /// Checks if an aggregate exists in the store.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TId">The type of the aggregate ID.</typeparam>
    /// <param name="aggregateId">The ID of the aggregate.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task indicating whether the aggregate exists.</returns>
    public Task<bool> Exists<TAggregate, TId>(
        TId aggregateId,
        CancellationToken cancellationToken = default)
        where TAggregate : class, IEventSourcedAggregate<TId>, new()
    {
        aggregateId.NotBeNull();

        var streamName = StreamName.For<TAggregate, TId>(aggregateId);

        return eventStore.StreamExists(streamName, cancellationToken);
    }
}