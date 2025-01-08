using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Core.Events;

/// <summary>
/// Represents a decorator for event handlers, providing additional behavior around the event handling process.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public class EventHandlerDecorator<TEvent>(IEventHandler<TEvent> eventHandler) 
    : IEventHandler<TEvent>
    where TEvent : IEvent
{
    /// <summary>
    /// Handles the event asynchronously and can be extended to add tracing or other behaviors.
    /// </summary>
    /// <param name="notification">The event to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(TEvent notification, CancellationToken cancellationToken)
    {
        // TODO: Using Activity for tracing
        await eventHandler.Handle(notification, cancellationToken);
    }
}