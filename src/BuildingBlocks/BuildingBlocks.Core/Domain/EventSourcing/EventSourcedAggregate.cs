using System.Collections.Concurrent;
using System.Collections.Immutable;
using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Abstractions.Domain.EventSourcing;
using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Core.Domain.Exceptions;

namespace BuildingBlocks.Core.Domain.EventSourcing;

/// <summary>
/// Represents an event-sourced aggregate root in the domain-driven design (DDD) pattern.
/// </summary>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
public abstract class EventSourcedAggregate<TId> : Entity<TId>, IEventSourcedAggregate<TId>
{
    [NonSerialized] private readonly ConcurrentQueue<IDomainEvent> _uncommittedDomainEvents = new();

    // -1: No Stream
    private const long NewAggregateVersion = -1;

    /// <summary>
    /// Gets the original version of the aggregate.
    /// </summary>
    public long OriginalVersion { get; private set; } = NewAggregateVersion;

    /// <summary>
    /// Gets the current version of the aggregate.
    /// </summary>
    public long CurrentVersion { get; private set; } = NewAggregateVersion;

    /// <summary>
    /// Applies a new event to the aggregate state, adds the event to the list of pending changes,
    /// and increments the `CurrentVersion`.
    /// </summary>
    /// <typeparam name="TDomainEvent">Type of domain event.</typeparam>
    /// <param name="domainEvent">The domain event to apply.</param>
    /// <param name="isNew">Indicates if the event is new.</param>
    protected virtual void ApplyEvent<TDomainEvent>(
        TDomainEvent domainEvent,
        bool isNew = true)
        where TDomainEvent : IDomainEvent
    {
        if (isNew)
        {
            AddDomainEvents(domainEvent);
        }

        When(domainEvent);
        CurrentVersion++;
    }

    /// <summary>
    /// Handles the event application logic.
    /// </summary>
    /// <param name="event">The event to apply.</param>
    public void When(object @event)
    {
        // with AggregateApply method checking
        // if (GetType().HasAggregateApplyMethod(@event.GetType()))
        // {
        //     this.InvokeMethod("Apply", @event);
        // }
        // else
        // {
        //     throw new AggregateException($"Can't find 'Apply' method for domain event: '{@event.GetType().Name}'");
        // }
    }

    /// <summary>
    /// Applies an event to the aggregate state and increments both `OriginalVersion` and `CurrentVersion`.
    /// </summary>
    /// <param name="event">The event to apply.</param>
    public void Fold(object @event)
    {
        When(@event);
        OriginalVersion++;
        CurrentVersion++;
    }

    /// <summary>
    /// Loads the aggregate state from a history of domain events.
    /// </summary>
    /// <param name="history">The history of domain events.</param>
    public void LoadFromHistory(IEnumerable<IDomainEvent> history)
    {
        history.ToList().ForEach(Fold);
    }

    /// <summary>
    /// Adds the specified domain event to the list of uncommitted domain events.
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
    /// Checks if there are any uncommitted domain events.
    /// </summary>
    /// <returns><c>true</c> if there are uncommitted domain events; otherwise, <c>false</c>.</returns>
    public bool HasUncommittedDomainEvents()
    {
        return !_uncommittedDomainEvents.IsEmpty;
    }

    /// <summary>
    /// Retrieves the list of uncommitted domain events.
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

        OriginalVersion = CurrentVersion;
    }

    /// <summary>
    /// Checks the specified business rule for any violations and throws an exception if the rule is broken.
    /// </summary>
    /// <param name="rule">The business rule to check.</param>
    public void CheckRule(IBusinessRule rule)
    {
        var broken = rule.IsBroken();
        if (broken)
        {
            throw new DomainException(rule.GetType(), rule.Message, rule.Status);
        }
    }

    /// <summary>
    /// Checks the specified business rule with a specific exception type for any violations and throws the specified exception if the rule is broken.
    /// </summary>
    /// <typeparam name="T">The type of the exception.</typeparam>
    /// <param name="ruleWithExceptionType">The business rule with exception type to check.</param>
    public void CheckRule<T>(IBusinessRuleWithExceptionType<T> ruleWithExceptionType)
        where T : DomainException
    {
        var isBroken = ruleWithExceptionType.IsBroken();
        if (isBroken)
        {
            throw ruleWithExceptionType.Exception;
        }
    }
}