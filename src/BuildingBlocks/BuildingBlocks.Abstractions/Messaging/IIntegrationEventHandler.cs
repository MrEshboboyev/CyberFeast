using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Abstractions.Messaging;

/// <summary>
/// Defines a handler for an integration event of type <typeparamref name="TEvent"/>.
/// </summary>
/// <typeparam name="TEvent">The type of the integration event.</typeparam>
public interface IIntegrationEventHandler<in TEvent> : IEventHandler<TEvent>
    where TEvent : IIntegrationEvent
{
}