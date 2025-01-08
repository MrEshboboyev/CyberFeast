using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Core.Messaging;
using MassTransit;

namespace BuildingBlocks.Core.Events.Internal;

/// <summary>
/// Wraps a domain event in an integration event for messaging purposes.
/// </summary>
/// <typeparam name="TDomainEventType">The type of the domain event.</typeparam>
[ExcludeFromTopology]
public record IntegrationEventWrapper<TDomainEventType> : IntegrationEvent
    where TDomainEventType : IDomainEvent;