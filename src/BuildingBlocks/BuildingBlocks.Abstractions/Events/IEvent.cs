using MediatR;

namespace BuildingBlocks.Abstractions.Events;

/// <summary>
/// Defines the structure of an event.
/// </summary>
public interface IEvent : INotification
{
    /// <summary>
    /// Gets the event identifier.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the event/aggregate root version.
    /// </summary>
    long EventVersion { get; }

    /// <summary>
    /// Gets the date the <see cref="IEvent"/> occurred on.
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Gets the timestamp of the event.
    /// </summary>
    DateTimeOffset TimeStamp { get; }

    /// <summary>
    /// Gets the type of this event.
    /// </summary>
    string EventType { get; }
}