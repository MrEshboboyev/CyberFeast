using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Core.Events;

/// <summary>
/// Provides storage for uncommitted domain events from aggregates.
/// </summary>
public class AggregatesDomainEventsStorage : IAggregatesDomainEventsRequestStorage
{
    private readonly List<IDomainEvent> _uncommittedDomainEvents = [];

    /// <summary>
    /// Adds events from the specified aggregate to the uncommitted domain events list.
    /// </summary>
    /// <typeparam name="T">The type of the aggregate.</typeparam>
    /// <param name="aggregate">The aggregate containing the events.</param>
    /// <returns>A read-only list of the added domain events.</returns>
    public IReadOnlyList<IDomainEvent> AddEventsFromAggregate<T>(T aggregate)
        where T : IHaveAggregate
    {
        var events = aggregate.GetUncommittedDomainEvents();

        AddEvents(events);

        return events;
    }

    /// <summary>
    /// Adds a list of domain events to the uncommitted domain events list.
    /// </summary>
    /// <param name="events">The list of domain events to add.</param>
    public void AddEvents(IReadOnlyList<IDomainEvent> events)
    {
        if (events.Any())
        {
            _uncommittedDomainEvents.AddRange(events);
        }
    }

    /// <summary>
    /// Returns all uncommitted domain events.
    /// </summary>
    /// <returns>A read-only list of all uncommitted domain events.</returns>
    public IReadOnlyList<IDomainEvent> GetAllUncommittedEvents()
    {
        return _uncommittedDomainEvents;
    }
}