namespace BuildingBlocks.Abstractions.Domain.EventSourcing;

/// <summary>
/// Defines a property for the current version of the event-sourced aggregate.
/// </summary>
public interface IHaveEventSourcedAggregateVersion : IHaveAggregateVersion
{
    /// <summary>
    /// Gets the current version of the aggregate. This is set to the original version when the aggregate is loaded from the store.
    /// It should increase for each state transition performed within the scope of the current operation.
    /// </summary>
    long CurrentVersion { get; }
}