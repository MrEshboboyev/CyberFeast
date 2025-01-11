using BuildingBlocks.Core.Events.Internal;
using BuildingBlocks.Persistence.EventStoreDB.Extensions;
using EventStore.Client;

namespace BuildingBlocks.Persistence.EventStoreDB.Subscriptions;

/// <summary>
/// Represents an event that stores a checkpoint for a subscription.
/// </summary>
public record CheckpointStored(
    string SubscriptionId,
    ulong? Position,
    DateTime CheckPointedAt) : DomainEvent;

/// <summary>
/// Implements a repository for storing and loading subscription checkpoints using EventStoreDB.
/// </summary>
public class EventStoreDbSubscriptionCheckPointRepository(EventStoreClient eventStoreClient)
    : ISubscriptionCheckpointRepository
{
    private readonly EventStoreClient _eventStoreClient =
        eventStoreClient ?? throw new ArgumentNullException(nameof(eventStoreClient));

    /// <summary>
    /// Loads the checkpoint position for the specified subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription ID.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The checkpoint position, or null if not found.</returns>
    public async ValueTask<ulong?> Load(
        string subscriptionId,
        CancellationToken ct)
    {
        var streamName = GetCheckpointStreamName(subscriptionId);

        var result = _eventStoreClient.ReadStreamAsync(
            Direction.Backwards,
            streamName,
            StreamPosition.End,
            1,
            cancellationToken: ct
        );

        if (await result.ReadState == ReadState.StreamNotFound)
        {
            return null;
        }

        ResolvedEvent? @event = await result.FirstOrDefaultAsync(ct);

        return @event?.DeserializeData<CheckpointStored>().Position;
    }

    /// <summary>
    /// Stores the checkpoint position for the specified subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription ID.</param>
    /// <param name="position">The checkpoint position.</param>
    /// <param name="ct">The cancellation token.</param>
    public async ValueTask Store(
        string subscriptionId,
        ulong position,
        CancellationToken ct)
    {
        var @event = new CheckpointStored(subscriptionId, position, DateTime.UtcNow);
        var eventToAppend = new[] { @event.ToJsonEventData() };
        var streamName = GetCheckpointStreamName(subscriptionId);

        try
        {
            // Store new checkpoint expecting stream to exist
            await _eventStoreClient.AppendToStreamAsync(
                streamName,
                StreamState.StreamExists,
                eventToAppend,
                cancellationToken: ct
            );
        }
        catch (WrongExpectedVersionException)
        {
            await _eventStoreClient.SetStreamMetadataAsync(
                streamName,
                StreamState.NoStream,
                new StreamMetadata(1),
                cancellationToken: ct
            );

            // Append event again expecting stream to not exist
            await _eventStoreClient.AppendToStreamAsync(
                streamName,
                StreamState.NoStream,
                eventToAppend,
                cancellationToken: ct
            );
        }
    }

    #region Private Methods

    /// <summary>
    /// Constructs the checkpoint stream name for the specified subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription ID.</param>
    /// <returns>The constructed checkpoint stream name.</returns>
    private static string GetCheckpointStreamName(string subscriptionId) =>
        $"checkpoint_{subscriptionId}";

    #endregion
}