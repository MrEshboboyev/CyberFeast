namespace BuildingBlocks.Abstractions.Persistence.EventStore;

/// <summary>
/// Represents an expected version of a stream in an event store.
/// </summary>
public record ExpectedStreamVersion(long Value)
{
    /// <summary>
    /// Represents a state where no stream exists.
    /// </summary>
    public static readonly ExpectedStreamVersion NoStream = new(-1);

    /// <summary>
    /// Represents a state where any stream version is acceptable.
    /// </summary>
    public static readonly ExpectedStreamVersion Any = new(-2);
}

/// <summary>
/// Represents the position from which to start reading a stream.
/// </summary>
public record StreamReadPosition(long Value)
{
    /// <summary>
    /// Represents the starting position of the stream.
    /// </summary>
    public static readonly StreamReadPosition Start = new(0L);
}

/// <summary>
/// Represents the position up to which a stream can be truncated.
/// </summary>
public record StreamTruncatePosition(long Value);