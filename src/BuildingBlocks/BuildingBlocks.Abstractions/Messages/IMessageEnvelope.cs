namespace BuildingBlocks.Abstractions.Messages;

/// <summary>
/// Represents a message envelope that contains a message of a specific type.
/// </summary>
/// <typeparam name="T">The type of message contained in the envelope, constrained to classes that implement the IMessage interface.</typeparam>
public interface IMessageEnvelope<out T> : IMessageEnvelopeBase
    where T : class, IMessage
{
    new T Message { get; }
}

// not required because of existing IMessageEnvelopeBase
// public interface IMessageEnvelope : IMessageEnvelope<object>;
