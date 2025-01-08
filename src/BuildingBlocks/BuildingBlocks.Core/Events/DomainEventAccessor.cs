using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Core.Events;

/// <summary>
/// Provides access to uncommitted domain events from the aggregates domain events storage and domain event context.
/// </summary>
public class DomainEventAccessor(
    IAggregatesDomainEventsRequestStorage aggregatesDomainEventsStorage,
    IDomainEventContext domainEventContext
) : IDomainEventsAccessor
{
    /// <summary>
    /// Gets the uncommitted domain events from either the aggregates domain events storage or domain event context.
    /// </summary>
    public IReadOnlyList<IDomainEvent> UnCommittedDomainEvents
    {
        get
        {
            var events = aggregatesDomainEventsStorage.GetAllUncommittedEvents();
            return events.Count != 0
                ? events
                : domainEventContext.GetAllUncommittedEvents();
        }
    }
}