using BuildingBlocks.Abstractions.Domain.EventSourcing;

namespace BuildingBlocks.Abstractions.Persistence.EventStore;

/// <summary>
/// Represents a store for managing aggregates, acting like a repository for the AggregateRoot.
/// </summary>
public interface IAggregateStore
{
    /// <summary>
    /// Loads the aggregate from the store using an aggregate ID.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TId">The type of the aggregate ID.</typeparam>
    /// <param name="aggregateId">The ID of the aggregate.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task with the aggregate as the result.</returns>
    Task<TAggregate?> GetAsync<TAggregate, TId>(TId aggregateId, CancellationToken cancellationToken = default)
        where TAggregate : class, IEventSourcedAggregate<TId>, new();

    /// <summary>
    /// Stores an aggregate state to the store using events (used for updating, adding, and deleting).
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TId">The type of the aggregate ID.</typeparam>
    /// <param name="aggregate">The aggregate object to be saved.</param>
    /// <param name="expectedVersion">The expected version saved from earlier. -1 if new.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task of the operation result.</returns>
    Task<AppendResult> StoreAsync<TAggregate, TId>(
        TAggregate aggregate,
        ExpectedStreamVersion? expectedVersion = null,
        CancellationToken cancellationToken = default)
        where TAggregate : class, IEventSourcedAggregate<TId>, new();

    /// <summary>
    /// Stores an aggregate state to the store using events (used for updating, adding, and deleting).
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TId">The type of the aggregate ID.</typeparam>
    /// <param name="aggregate">The aggregate object to be saved.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task of the operation result.</returns>
    Task<AppendResult> StoreAsync<TAggregate, TId>(TAggregate aggregate, CancellationToken cancellationToken = default)
        where TAggregate : class, IEventSourcedAggregate<TId>, new();

    /// <summary>
    /// Checks if an aggregate exists in the store.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TId">The type of the aggregate ID.</typeparam>
    /// <param name="aggregateId">The ID of the aggregate.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A task indicating whether the aggregate exists.</returns>
    Task<bool> Exists<TAggregate, TId>(TId aggregateId, CancellationToken cancellationToken = default)
        where TAggregate : class, IEventSourcedAggregate<TId>, new();
}