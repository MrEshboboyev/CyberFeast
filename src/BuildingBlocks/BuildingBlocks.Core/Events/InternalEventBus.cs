using System.Collections.Concurrent;
using System.Reflection;
using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Persistence.EventStore;
using MediatR;
using Polly;

namespace BuildingBlocks.Core.Events;

/// <summary>
/// Represents an internal event bus for publishing events.
/// </summary>
public class InternalEventBus(IMediator mediator, AsyncPolicy policy) : IInternalEventBus
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> _publishMethods = new();

    /// <summary>
    /// Publishes an event asynchronously.
    /// </summary>
    /// <param name="event">The event to publish.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Publish(IEvent @event, CancellationToken ct)
    {
        var retryAsync = Policy.Handle<System.Exception>().RetryAsync(2);

        await retryAsync.ExecuteAsync(c => mediator.Publish(@event, c), ct);
    }

    /// <summary>
    /// Publishes a collection of events asynchronously.
    /// </summary>
    /// <param name="eventsData">The collection of events to publish.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Publish(IEnumerable<IEvent> eventsData, CancellationToken ct)
    {
        foreach (var eventData in eventsData)
        {
            await Publish(eventData, ct).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Publishes an event envelope asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the event message.</typeparam>
    /// <param name="eventEnvelope">The event envelope to publish.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Publish<T>(IEventEnvelope<T> eventEnvelope, CancellationToken ct)
        where T : class
    {
        await policy.ExecuteAsync(
            c =>
            {
                // TODO: using metadata for tracing and monitoring here
                return mediator.Publish(eventEnvelope.Message, c);
            },
            ct
        );
    }

    /// <summary>
    /// Publishes an event envelope asynchronously.
    /// </summary>
    /// <param name="eventEnvelope">The event envelope to publish.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task Publish(IEventEnvelope eventEnvelope, CancellationToken ct)
    {
        // Calling generic `Publish<T>` in `InternalEventBus` class
        var genericPublishMethod = _publishMethods.GetOrAdd(
            eventEnvelope.Message.GetType(),
            eventType =>
                typeof(InternalEventBus)
                    .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .First(m => m.Name == nameof(Publish) && m.GetGenericArguments().Length != 0)
                    .MakeGenericMethod(eventType)
        );

        return (Task)genericPublishMethod.Invoke(this, new object[] { eventEnvelope, ct })!;
    }

    /// <summary>
    /// Publishes a stream event envelope asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the event message.</typeparam>
    /// <param name="streamEvent">The stream event envelope to publish.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Publish<T>(IStreamEventEnvelope<T> streamEvent, CancellationToken ct)
        where T : IDomainEvent
    {
        await policy.ExecuteAsync(
            c =>
            {
                // TODO: using metadata for tracing and monitoring here
                return mediator.Publish(streamEvent.Data, c);
            },
            ct
        );
    }

    /// <summary>
    /// Publishes a stream event envelope asynchronously.
    /// </summary>
    /// <param name="streamEvent">The stream event envelope to publish.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task Publish(IStreamEventEnvelope streamEvent, CancellationToken ct)
    {
        // Calling generic `Publish<T>` in `InternalEventBus` class
        var genericPublishMethod = _publishMethods.GetOrAdd(
            streamEvent.Data.GetType(),
            eventType =>
                typeof(InternalEventBus)
                    .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .First(m => m.Name == nameof(Publish) && m.GetGenericArguments().Length != 0)
                    .MakeGenericMethod(eventType)
        );

        return (Task)genericPublishMethod.Invoke(this, new object[] { streamEvent, ct })!;
    }
}