using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Core.Events;

/// <summary>
/// Provides a null implementation of <see cref="IDomainEventsAccessor"/> that returns an empty list of uncommitted domain events.
/// </summary>
public class NullDomainEventsAccessor : IDomainEventsAccessor
{
    /// <summary>
    /// Gets an empty list of uncommitted domain events.
    /// </summary>
    public IReadOnlyList<IDomainEvent> UnCommittedDomainEvents { get; } = new List<IDomainEvent>();
}