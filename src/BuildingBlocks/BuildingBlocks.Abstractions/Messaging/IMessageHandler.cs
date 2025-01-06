using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Abstractions.Messaging;

/// <summary>
/// Defines a handler for a message of type <typeparamref name="TMessage"/>.
/// </summary>
/// <typeparam name="TMessage">The type of the message.</typeparam>
public interface IMessageHandler<in TMessage>
    where TMessage : IMessage
{
    /// <summary>
    /// Asynchronously handles the message.
    /// </summary>
    /// <param name="eventEnvelope">The event envelope containing the message.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleAsync(
        IEventEnvelope<TMessage> eventEnvelope,
        CancellationToken cancellationToken = default);
}