using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Abstractions.Persistence.EventStore.Projections;

/// <summary>
/// Defines methods for publishing read projections based on stream events.
/// </summary>
public interface IReadProjectionPublisher
{
    /// <summary>
    /// Asynchronously publishes the read projection based on the provided stream event envelope.
    /// </summary>
    /// <param name="streamEvent">The stream event envelope containing the event and its metadata.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync(
        IStreamEventEnvelope streamEvent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously publishes the read projection based on the provided stream event envelope of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the domain event.</typeparam>
    /// <param name="streamEvent">The stream event envelope containing the domain event and its metadata.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync<T>(
        IStreamEventEnvelope<T> streamEvent,
        CancellationToken cancellationToken = default)
        where T : IDomainEvent;
}