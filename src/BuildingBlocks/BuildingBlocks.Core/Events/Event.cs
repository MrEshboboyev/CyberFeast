using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Core.Types;

namespace BuildingBlocks.Core.Events;

/// <summary>
/// Represents an event with properties like EventId, EventVersion, OccurredOn, TimeStamp, and EventType.
/// </summary>
public abstract record Event : IEvent
{
    /// <summary>
    /// Gets or sets the unique identifier for the event.
    /// </summary>
    public Guid EventId { get; protected set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the version of the event.
    /// </summary>
    public long EventVersion { get; protected set; } = -1;

    /// <summary>
    /// Gets or sets the timestamp when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; protected set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the timestamp with offset when the event occurred.
    /// </summary>
    public DateTimeOffset TimeStamp { get; protected set; } = DateTimeOffset.Now;

    /// <summary>
    /// Gets the type of the event.
    /// </summary>
    public string EventType => TypeMapper.GetFullTypeName(GetType());
}