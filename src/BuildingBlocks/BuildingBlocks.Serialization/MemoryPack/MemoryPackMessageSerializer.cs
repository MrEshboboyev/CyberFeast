using System.Text;
using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Serialization;
using BuildingBlocks.Core.Events;
using MemoryPack;

namespace BuildingBlocks.Serialization.MemoryPack;

/// <summary>
/// Provides methods for serializing and deserializing message envelopes using MemoryPack.
/// </summary>
public class MemoryPackMessageSerializer(MemoryPackSerializerOptions options) : IMessageSerializer
{
    /// <summary>
    /// Gets the content type of the serialized data.
    /// </summary>
    public string ContentType => "binary/memorypack";

    /// <summary>
    /// Serializes an event envelope to a UTF-8 encoded string.
    /// </summary>
    /// <param name="eventEnvelope">The event envelope to serialize.</param>
    /// <returns>The serialized event envelope as a string.</returns>
    public string Serialize(IEventEnvelope eventEnvelope)
    {
        return Encoding.UTF8.GetString(MemoryPackSerializer.Serialize(eventEnvelope, options));
    }

    /// <summary>
    /// Serializes a typed event envelope to a UTF-8 encoded string.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="eventEnvelope">The typed event envelope to serialize.</param>
    /// <returns>The serialized event envelope as a string.</returns>
    public string Serialize<T>(IEventEnvelope<T> eventEnvelope)
        where T : IMessage
    {
        return Encoding.UTF8.GetString(MemoryPackSerializer.Serialize(eventEnvelope, options));
    }

    /// <summary>
    /// Deserializes a string to an event envelope of the specified message type.
    /// </summary>
    /// <param name="eventEnvelope">The serialized event envelope as a string.</param>
    /// <param name="messageType">The message type.</param>
    /// <returns>The deserialized event envelope.</returns>
    public IEventEnvelope? Deserialize(string eventEnvelope, Type messageType)
    {
        var eventEnvelopeType = typeof(EventEnvelope<>).MakeGenericType(messageType);
        var byteSpan = StringToReadOnlySpan(eventEnvelope);
        return MemoryPackSerializer.Deserialize(eventEnvelopeType, byteSpan, options) as IEventEnvelope;
    }

    /// <summary>
    /// Deserializes a string to a typed event envelope.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="eventEnvelope">The serialized event envelope as a string.</param>
    /// <returns>The deserialized typed event envelope.</returns>
    public IEventEnvelope<T>? Deserialize<T>(string eventEnvelope)
        where T : IMessage
    {
        var byteSpan = StringToReadOnlySpan(eventEnvelope);
        return MemoryPackSerializer.Deserialize<EventEnvelope<T>>(byteSpan, options);
    }

    /// <summary>
    /// Converts a string to a <see cref="ReadOnlySpan{T}"/> of bytes.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>A read-only span of bytes representing the string.</returns>
    private static ReadOnlySpan<byte> StringToReadOnlySpan(string input)
    {
        // Choose the encoding
        var encoding = Encoding.UTF8;

        // Convert the string to a byte array
        var byteArray = encoding.GetBytes(input);

        // Return a ReadOnlySpan<byte> from the byte array
        return new ReadOnlySpan<byte>(byteArray);
    }
}