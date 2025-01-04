namespace BuildingBlocks.Abstractions.Persistence.EventStore;

/// <summary>
/// Represents the result of appending events to a stream in an event store.
/// </summary>
public record AppendResult(long GlobalPosition, long NextExpectedVersion)
{
    /// <summary>
    /// Represents a default or empty append result.
    /// </summary>
    public static readonly AppendResult None = new(0, -1);
}