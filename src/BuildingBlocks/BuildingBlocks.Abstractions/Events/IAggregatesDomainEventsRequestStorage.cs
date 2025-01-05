using BuildingBlocks.Abstractions.Domain;

namespace BuildingBlocks.Abstractions.Events;

/// <summary>
/// Defines methods for managing domain events within an aggregate.
/// </summary>
public interface IAggregatesDomainEventsRequestStorage
{
    /// <summary>
    /// Adds events from the specified aggregate.
    /// </summary>
    /// <typeparam name="T">The type of the aggregate.</typeparam>
    /// <param name="aggregate">The aggregate to add events from.</param>
    /// <returns>A read-only list of domain events.</returns>
    IReadOnlyList<IDomainEvent> AddEventsFromAggregate<T>(T aggregate)
        where T : IHaveAggregate;

    /// <summary>
    /// Adds a list of domain events.
    /// </summary>
    /// <param name="events">The list of domain events to add.</param>
    void AddEvents(IReadOnlyList<IDomainEvent> events);

    /// <summary>
    /// Retrieves all uncommitted domain events.
    /// </summary>
    /// <returns>A read-only list of uncommitted domain events.</returns>
    IReadOnlyList<IDomainEvent> GetAllUncommittedEvents();
}