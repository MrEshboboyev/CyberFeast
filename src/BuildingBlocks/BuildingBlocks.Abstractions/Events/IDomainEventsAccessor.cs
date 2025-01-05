namespace BuildingBlocks.Abstractions.Events;

/// <summary>
/// Defines a property for accessing uncommitted domain events.
/// </summary>
public interface IDomainEventsAccessor
{
    /// <summary>
    /// Gets a read-only list of uncommitted domain events.
    /// </summary>
    IReadOnlyList<IDomainEvent> UnCommittedDomainEvents { get; }
}