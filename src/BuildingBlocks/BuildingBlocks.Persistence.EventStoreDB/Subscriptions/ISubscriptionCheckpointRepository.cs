namespace BuildingBlocks.Persistence.EventStoreDB.Subscriptions;

/// <summary>
/// Defines methods for loading and storing subscription checkpoints.
/// </summary>
public interface ISubscriptionCheckpointRepository
{
    /// <summary>
    /// Loads the checkpoint position for the specified subscription asynchronously.
    /// </summary>
    /// <param name="subscriptionId">The subscription ID.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>The checkpoint position, or null if no checkpoint is found.</returns>
    ValueTask<ulong?> Load(string subscriptionId, CancellationToken ct);

    /// <summary>
    /// Stores the checkpoint position for the specified subscription asynchronously.
    /// </summary>
    /// <param name="subscriptionId">The subscription ID.</param>
    /// <param name="position">The checkpoint position.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    ValueTask Store(string subscriptionId, ulong position, CancellationToken ct);
}