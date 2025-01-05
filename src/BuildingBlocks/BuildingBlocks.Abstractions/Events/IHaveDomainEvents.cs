namespace BuildingBlocks.Abstractions.Events;

/// <summary>
/// Defines methods for managing domain events within an aggregate.
/// </summary>
public interface IHaveDomainEvents
{
    /// <summary>
    /// Checks if the aggregate has changes that have not been committed to storage.
    /// </summary>
    /// <returns>A boolean value indicating whether there are uncommitted domain events.</returns>
    bool HasUncommittedDomainEvents();

    /// <summary>
    /// Gets a list of uncommitted events for this aggregate.
    /// </summary>
    /// <returns>A read-only list of uncommitted domain events.</returns>
    IReadOnlyList<IDomainEvent> GetUncommittedDomainEvents();

    /// <summary>
    /// Removes all domain events.
    /// </summary>
    void ClearDomainEvents();

    /// <summary>
    /// Marks all changes (events) as committed, clears uncommitted changes, and updates the current version of the aggregate.
    /// </summary>
    void MarkUncommittedDomainEventAsCommitted();
}