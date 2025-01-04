namespace BuildingBlocks.Abstractions.Persistence.EventStore.Projections;

/// <summary>
/// Defines methods for updating and restoring the state of an aggregate in an event store.
/// </summary>
public interface IHaveAggregateStateProjection
{
    /// <summary>
    /// Updates the aggregate state with new events that are added to the event store and also for events that are already
    /// in the event store without increasing the version.
    /// </summary>
    /// <param name="event">The event to apply to the aggregate state.</param>
    void When(object @event);

    /// <summary>
    /// Restores the aggregate state with events that are loaded from the event store and increases the current version and
    /// last commit version.
    /// </summary>
    /// <param name="event">The event to apply to the aggregate state.</param>
    void Fold(object @event);
}