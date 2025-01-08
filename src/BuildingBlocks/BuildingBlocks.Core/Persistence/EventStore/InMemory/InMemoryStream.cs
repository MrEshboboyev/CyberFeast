using BuildingBlocks.Abstractions.Persistence.EventStore;

namespace BuildingBlocks.Core.Persistence.EventStore.InMemory;

/// <summary>
/// Represents an in-memory stream in the event store.
/// </summary>
public class InMemoryStream
{
    private readonly List<StreamEventData> _events = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryStream"/> class.
    /// </summary>
    /// <param name="name">The name of the stream.</param>
    public InMemoryStream(string name)
    {
        StreamName = name;
    }

    /// <summary>
    /// Gets the current version of the stream.
    /// </summary>
    public int Version { get; private set; } = -1;

    /// <summary>
    /// Gets the name of the stream.
    /// </summary>
    public string StreamName { get; }

    /// <summary>
    /// Checks if the expected stream version matches the actual stream version.
    /// </summary>
    /// <param name="expectedVersion">The expected stream version.</param>
    /// <exception cref="System.Exception">Thrown when the expected version does not match the actual version.</exception>
    public void CheckVersion(ExpectedStreamVersion expectedVersion)
    {
        if (
            (expectedVersion.Value == ExpectedStreamVersion.NoStream.Value &&
             _events.Count != 0 == false)
            || expectedVersion.Value == ExpectedStreamVersion.Any.Value
        ) return;

        if (expectedVersion.Value != Version)
            throw new System.Exception($"Wrong stream version. Expected {expectedVersion.Value}, actual {Version}");
    }

    /// <summary>
    /// Appends events to the stream.
    /// </summary>
    /// <param name="expectedVersion">The expected stream version.</param>
    /// <param name="globalAllPosition">The global position of the last event in the stream.</param>
    /// <param name="events">The events to append.</param>
    public void AppendEvents(
        ExpectedStreamVersion expectedVersion,
        int globalAllPosition,
        IReadOnlyCollection<StreamEventData> events)
    {
        CheckVersion(expectedVersion);

        foreach (var @event in events)
        {
            var version = ++Version;
            @event.StreamId = StreamName;
            @event.Name = $"{version}@{StreamName}";
            @event.Timestamp = DateTime.Now;
            @event.EventNumber = version;
            @event.GlobalEventPosition = globalAllPosition + 1;
        }

        _events.AddRange(events);
    }

    /// <summary>
    /// Retrieves events from the stream starting from the specified position.
    /// </summary>
    /// <param name="from">The position from which to start reading events.</param>
    /// <param name="count">The maximum number of events to retrieve.</param>
    /// <returns>An enumerable of stream event data.</returns>
    public IEnumerable<StreamEventData> GetEvents(StreamReadPosition from, int count)
    {
        var selected = _events
            .SkipWhile(x => x.GlobalEventPosition < from.Value);

        if (count > 0)
            selected = selected.Take(count);

        return selected;
    }

    /// <summary>
    /// Retrieves events from the stream in reverse order.
    /// </summary>
    /// <param name="count">The maximum number of events to retrieve.</param>
    /// <returns>An enumerable of stream event data.</returns>
    public IEnumerable<StreamEventData> GetEventsBackwards(int count)
    {
        var position = _events.Count - 1;

        while (count-- > 0)
        {
            yield return _events[position--];
        }
    }
}