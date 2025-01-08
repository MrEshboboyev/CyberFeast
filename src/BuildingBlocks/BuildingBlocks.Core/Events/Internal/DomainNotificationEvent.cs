using BuildingBlocks.Abstractions.Events.Internal;

namespace BuildingBlocks.Core.Events.Internal;

/// <summary>
/// Represents a domain notification event that can be used for notifications within the domain.
/// </summary>
public abstract record DomainNotificationEvent : Event, IDomainNotificationEvent;