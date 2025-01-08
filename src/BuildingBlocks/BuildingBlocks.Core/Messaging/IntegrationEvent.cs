using BuildingBlocks.Abstractions.Messaging;

namespace BuildingBlocks.Core.Messaging;

/// <summary>
/// Represents an integration event, which is a type of message used for integration purposes.
/// </summary>
public abstract record IntegrationEvent : Message, IIntegrationEvent;