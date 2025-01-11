using System.Collections.Concurrent;

namespace BuildingBlocks.Persistence.EventStoreDB.Subscriptions;

/// <summary>
/// An in-memory implementation of the <see cref="ISubscriptionCheckpointRepository"/> for storing subscription checkpoints.
/// </summary>
public class InMemorySubscriptionCheckpointRepository : ISubscriptionCheckpointRepository
{
    private readonly ConcurrentDictionary<string, ulong> _checkpoints = new();

    /// <summary>
    /// Loads the checkpoint position for the specified subscription from the in-memory store.
    /// </summary>
    /// <param name="subscriptionId">The subscription ID.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The checkpoint position, or null if not found.</returns>
    public ValueTask<ulong?> Load(string subscriptionId, CancellationToken ct)
    {
        return new ValueTask<ulong?>(
            _checkpoints.TryGetValue(subscriptionId, out var checkpoint)
                ? checkpoint
                : null);
    }

    /// <summary>
    /// Stores the checkpoint position for the specified subscription in the in-memory store.
    /// </summary>
    /// <param name="subscriptionId">The subscription ID.</param>
    /// <param name="position">The checkpoint position.</param>
    /// <param name="ct">The cancellation token.</param>
    public ValueTask Store(string subscriptionId, ulong position, CancellationToken ct)
    {
        _checkpoints.AddOrUpdate(subscriptionId, position, (_, _) => position);

        return ValueTask.CompletedTask;
    }
}