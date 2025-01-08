namespace BuildingBlocks.Abstractions.Events;

/// <summary>
/// Defines the structure of a domain event.
/// </summary>
public interface IDomainEvent : IEvent
{
    /// <summary>
    /// Gets the identifier of the aggregate which has generated the event.
    /// </summary>
    dynamic? AggregateId { get; }

    /// <summary>
    /// Gets the sequence number of the event within the aggregate.
    /// </summary>
    long AggregateSequenceNumber { get; }

    /// <summary>
    /// Returns a new domain event instance with the specified aggregate identifier and version.
    /// </summary>
    /// <param name="aggregateId">The identifier of the aggregate.</param>
    /// <param name="version">The version of the event.</param>
    /// <returns>A new domain event instance.</returns>
    IDomainEvent WithAggregate(dynamic? aggregateId, long version);
}