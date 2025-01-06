using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Abstractions.Persistence.EventStore.Projections;

/// <summary>
/// Defines a contract for read projections related to domain events.
/// </summary>
public interface IHaveReadProjection
{
    /// <summary>
    /// Asynchronously projects the read model based on the provided stream event envelope.
    /// </summary>
    /// <typeparam name="T">The type of the domain event.</typeparam>
    /// <param name="streamEvent">The stream event envelope containing the domain event and its metadata.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ProjectAsync<T>(
        IStreamEventEnvelope<T> streamEvent,
        CancellationToken cancellationToken = default)
        where T : IDomainEvent;
}