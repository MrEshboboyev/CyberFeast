using BuildingBlocks.Abstractions.Persistence.EventStore;

namespace BuildingBlocks.Core.Persistence.EventStore;

/// <summary>
/// Represents the metadata for an event in the event stream.
/// </summary>
/// <param name="EventId">The unique event identifier.</param>
/// <param name="StreamPosition">The position of the event in the stream.</param>
/// <param name="LogPosition">The position of the event in the log (optional).</param>
public record StreamEventMetadata(
    string EventId,
    ulong StreamPosition,
    ulong? LogPosition) : IStreamEventMetadata;