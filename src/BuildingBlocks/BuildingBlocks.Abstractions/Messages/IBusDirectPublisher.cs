using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Abstractions.Messages;

/// <summary>
/// Defines methods for direct publishing of messages.
/// </summary>
public interface IBusDirectPublisher
{
    /// <summary>
    /// Asynchronously publishes an event envelope directly.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="eventEnvelope">The event envelope to publish.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync<TMessage>(
        IEventEnvelope<TMessage> eventEnvelope,
        CancellationToken cancellationToken = default)
        where TMessage : IMessage;

    /// <summary>
    /// Asynchronously publishes an event envelope directly.
    /// </summary>
    /// <param name="eventEnvelope">The event envelope to publish.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync(
        IEventEnvelope eventEnvelope,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously publishes an event envelope directly with a specified exchange or topic and queue.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="eventEnvelope">The event envelope to publish.</param>
    /// <param name="exchangeOrTopic">The exchange or topic to publish the message to.</param>
    /// <param name="queue">The queue to publish the message to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync<TMessage>(
        IEventEnvelope<TMessage> eventEnvelope,
        string? exchangeOrTopic = null,
        string? queue = null,
        CancellationToken cancellationToken = default)
        where TMessage : IMessage;

    /// <summary>
    /// Asynchronously publishes an event envelope directly with a specified exchange or topic and queue.
    /// </summary>
    /// <param name="eventEnvelope">The event envelope to publish.</param>
    /// <param name="exchangeOrTopic">The exchange or topic to publish the message to.</param>
    /// <param name="queue">The queue to publish the message to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync(
        IEventEnvelope eventEnvelope,
        string? exchangeOrTopic = null,
        string? queue = null,
        CancellationToken cancellationToken = default);
}
