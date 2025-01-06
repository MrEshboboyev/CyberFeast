using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Messaging;

namespace BuildingBlocks.Abstractions.Serialization;

/// <summary>
/// Defines methods for serializing and deserializing messages encapsulated in event envelopes.
/// </summary>
public interface IMessageSerializer
{
    /// <summary>
    /// Gets the content type used by the message serializer.
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// Serializes the given <see cref="IEventEnvelope"/> into a string.
    /// </summary>
    /// <param name="eventEnvelope">The event envelope to serialize.</param>
    /// <returns>A string representation of the serialized event envelope.</returns>
    string Serialize(IEventEnvelope eventEnvelope);

    /// <summary>
    /// Serializes the given <see cref="IEventEnvelope{T}"/> into a string.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="eventEnvelope">The event envelope to serialize.</param>
    /// <returns>A string representation of the serialized event envelope.</returns>
    string Serialize<T>(IEventEnvelope<T> eventEnvelope)
        where T : IMessage;

    /// <summary>
    /// Deserializes the given string into an <see cref="IEventEnvelope"/>.
    /// </summary>
    /// <param name="eventEnvelope">The string representation of the event envelope.</param>
    /// <param name="messageType">The type of the message inside the event envelope.</param>
    /// <returns>The deserialized event envelope.</returns>
    IEventEnvelope? Deserialize(string eventEnvelope, Type messageType);

    /// <summary>
    /// Deserializes the given string into an <see cref="IEventEnvelope{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="eventEnvelope">The string representation of the event envelope.</param>
    /// <returns>The deserialized event envelope.</returns>
    IEventEnvelope<T>? Deserialize<T>(string eventEnvelope)
        where T : IMessage;
}