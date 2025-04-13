namespace BuildingBlocks.Abstractions.Messages;

/// <summary>
/// Defines an external event bus that provides both publishing and consuming capabilities.
/// </summary>
public interface IExternalEventBus : IBusPublisher, IBusConsumer
{
}