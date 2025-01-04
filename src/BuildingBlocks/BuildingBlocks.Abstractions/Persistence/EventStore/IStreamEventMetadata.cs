namespace BuildingBlocks.Abstractions.Persistence.EventStore;

/// <summary>
/// Defines metadata for stream events in an event store.
/// </summary>
public interface IStreamEventMetadata
{
    /// <summary>
    /// Gets the unique identifier of the event.
    /// </summary>
    string EventId { get; }

    /// <summary>
    /// Gets the log position of the event (optional).
    /// </summary>
    ulong? LogPosition { get; }

    /// <summary>
    /// Gets the position of the event within the stream.
    /// </summary>
    ulong StreamPosition { get; }
}