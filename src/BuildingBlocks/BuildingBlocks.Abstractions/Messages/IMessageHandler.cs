namespace BuildingBlocks.Abstractions.Messages;

/// <summary>
/// Defines a handler for a message of type <typeparamref name="TMessage"/>.
/// </summary>
/// <typeparam name="TMessage">The type of the message.</typeparam>
public interface IMessageHandler<in TMessage>
    where TMessage : class, IMessage
{
    /// <summary>
    /// Asynchronously handles the message.
    /// </summary>
    /// <param name="message">The message to handle.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}
