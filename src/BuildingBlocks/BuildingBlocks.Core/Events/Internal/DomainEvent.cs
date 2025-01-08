using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Core.Events.Internal;

/// <summary>
/// Represents a domain event with properties like AggregateId and AggregateSequenceNumber.
/// </summary>
public abstract record DomainEvent : Event, IDomainEvent
{
    /// <summary>
    /// Gets or sets the ID of the aggregate that raised the event.
    /// </summary>
    public dynamic? AggregateId { get; protected set; } = null;

    /// <summary>
    /// Gets or sets the sequence number of the event within the aggregate.
    /// </summary>
    public long AggregateSequenceNumber { get; protected set; }

    /// <summary>
    /// Associates the event with an aggregate ID and sequence number.
    /// </summary>
    /// <param name="aggregateId">The ID of the aggregate.</param>
    /// <param name="version">The sequence number of the event.</param>
    /// <returns>The updated domain event.</returns>
    public virtual IDomainEvent WithAggregate(dynamic? aggregateId, long version)
    {
        AggregateId = aggregateId;
        AggregateSequenceNumber = version;

        return this;
    }
}