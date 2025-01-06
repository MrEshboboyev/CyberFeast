using System.Collections.Concurrent;
using System.Collections.Immutable;
using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Core.Domain;

/// <summary>
/// Represents an aggregate root in the domain-driven design (DDD) pattern.
/// </summary>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
public abstract class Aggregate<TId> : Entity<TId>, IAggregate<TId>
{
    [NonSerialized] private readonly ConcurrentQueue<IDomainEvent> _uncommittedDomainEvents = new();

    private const long NewAggregateVersion = 0;

    public long OriginalVersion { get; private set; } = NewAggregateVersion;

    /// <summary>
    /// Adds the specified domain event to the aggregate's pending changes.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    protected void AddDomainEvents(IDomainEvent domainEvent)
    {
        if (!_uncommittedDomainEvents.Any(x => Equals(x.EventId, domainEvent.EventId)))
        {
            _uncommittedDomainEvents.Enqueue(domainEvent);
        }
    }

    /// <summary>
    /// Determines whether the aggregate has uncommitted domain events.
    /// </summary>
    /// <returns><c>true</c> if there are uncommitted domain events; otherwise, <c>false</c>.</returns>
    public bool HasUncommittedDomainEvents()
    {
        return !_uncommittedDomainEvents.IsEmpty;
    }

    /// <summary>
    /// Gets the list of uncommitted domain events.
    /// </summary>
    /// <returns>A read-only list of uncommitted domain events.</returns>
    public IReadOnlyList<IDomainEvent> GetUncommittedDomainEvents()
    {
        return _uncommittedDomainEvents.ToImmutableList();
    }

    /// <summary>
    /// Clears all uncommitted domain events.
    /// </summary>
    public void ClearDomainEvents()
    {
        _uncommittedDomainEvents.Clear();
    }

    /// <summary>
    /// Dequeues and retrieves the list of uncommitted domain events.
    /// </summary>
    /// <returns>A read-only list of uncommitted domain events.</returns>
    public IReadOnlyList<IDomainEvent> DequeueUncommittedDomainEvents()
    {
        var events = _uncommittedDomainEvents.ToImmutableList();
        MarkUncommittedDomainEventAsCommitted();
        return events;
    }

    /// <summary>
    /// Marks all uncommitted domain events as committed.
    /// </summary>
    public void MarkUncommittedDomainEventAsCommitted()
    {
        _uncommittedDomainEvents.Clear();
    }

    /// <summary>
    /// Checks the specified business rule for any violations.
    /// </summary>
    /// <param name="rule">The business rule to check.</param>
    public void CheckRule(IBusinessRule rule)
    {
        var isBroken = rule.IsBroken();
        if (isBroken)
        {
            // throw DomainException
        }
    }

    // CheckRule with Custom Domain Exception
}

/// <summary>
/// Represents an aggregate root with a specified identity type and identifier type.
/// </summary>
/// <typeparam name="TIdentity">The type of the aggregate identity.</typeparam>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
public abstract class Aggregate<TIdentity, TId> : Aggregate<TIdentity>
    where TIdentity : Identity<TId>
{
}

/// <summary>
/// Represents an aggregate root with a default identity type and identifier type.
/// </summary>
public abstract class Aggregate : Aggregate<AggregateId, long>, IAggregate
{
}