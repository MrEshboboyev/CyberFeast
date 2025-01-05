namespace BuildingBlocks.Abstractions.Events;

/// <summary>
/// Defines methods for managing uncommitted domain events.
/// </summary>
public interface IDomainEventContext
{
    /// <summary>
    /// Retrieves all uncommitted domain events.
    /// </summary>
    /// <returns>A read-only list of uncommitted domain events.</returns>
    IReadOnlyList<IDomainEvent> GetAllUncommittedEvents();

    /// <summary>
    /// Marks all uncommitted domain events as committed.
    /// </summary>
    void MarkUncommittedDomainEventAsCommitted();
}