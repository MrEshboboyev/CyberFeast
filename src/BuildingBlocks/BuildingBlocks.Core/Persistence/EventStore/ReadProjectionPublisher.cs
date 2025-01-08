using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Persistence.EventStore;
using BuildingBlocks.Abstractions.Persistence.EventStore.Projections;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Persistence.EventStore;

/// <summary>
/// Publishes read projections asynchronously.
/// </summary>
public class ReadProjectionPublisher(
    IServiceProvider serviceProvider) : IReadProjectionPublisher
{
    /// <summary>
    /// Publishes a stream event to all registered read projections for a specific domain event type.
    /// </summary>
    /// <typeparam name="T">The type of the domain event.</typeparam>
    /// <param name="streamEvent">The stream event envelope containing the domain event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PublishAsync<T>(
        IStreamEventEnvelope<T> streamEvent,
        CancellationToken cancellationToken = default) where T : IDomainEvent
    {
        using var scope = serviceProvider.CreateScope();
        var projections = scope.ServiceProvider.GetRequiredService<IEnumerable<IHaveReadProjection>>();
        foreach (var projection in projections)
        {
            await projection.ProjectAsync(streamEvent, cancellationToken);
        }
    }

    /// <summary>
    /// Dynamically invokes the generic <see cref="PublishAsync{T}"/> method based on the event data type.
    /// </summary>
    /// <param name="streamEvent">The stream event envelope containing the domain event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task PublishAsync(
        IStreamEventEnvelope streamEvent,
        CancellationToken cancellationToken = default)
    {
        var streamData = streamEvent.Data.GetType();

        var method = typeof(IReadProjectionPublisher)
            .GetMethods()
            .First(m => m.Name == nameof(PublishAsync)
                        && m.GetGenericArguments().Length != 0)
            .MakeGenericMethod(streamData);

        return (Task)method.Invoke(this, new object[] { streamEvent, cancellationToken })!;
    }
}