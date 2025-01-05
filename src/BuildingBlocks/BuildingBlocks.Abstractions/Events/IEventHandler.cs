using MediatR;

namespace BuildingBlocks.Abstractions.Events;

/// <summary>
/// Defines the structure of an event handler.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : INotification
{
}