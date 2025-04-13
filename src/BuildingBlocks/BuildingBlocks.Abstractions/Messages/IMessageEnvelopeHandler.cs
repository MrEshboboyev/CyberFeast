namespace BuildingBlocks.Abstractions.Messages;

/// <summary>
/// Handles message envelopes asynchronously, processing the contained message.
/// </summary>
/// <typeparam name="TMessage">Represents a specific type of message that can be processed within the envelope.</typeparam>
public interface IMessageEnvelopeHandler<in TMessage>
    where TMessage : class, IMessage
{
    Task HandleAsync(IMessageEnvelope<TMessage> messageEnvelope, CancellationToken cancellationToken = default);
}
