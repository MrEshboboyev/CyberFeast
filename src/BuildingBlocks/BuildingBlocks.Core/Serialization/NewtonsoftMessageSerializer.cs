using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Serialization;
using BuildingBlocks.Core.Events;
using Newtonsoft.Json;

namespace BuildingBlocks.Core.Serialization;

/// <summary>
/// Message serializer implementation using Newtonsoft.Json.
/// </summary>
public class NewtonsoftMessageSerializer(JsonSerializerSettings settings) : IMessageSerializer
{
    /// <summary>
    /// Gets the content type for the serialized messages.
    /// </summary>
    public string ContentType => "application/json";

    /// <summary>
    /// Serializes an event envelope to a JSON string.
    /// </summary>
    public string Serialize(IEventEnvelope eventEnvelope)
    {
        return JsonConvert.SerializeObject(eventEnvelope, settings);
    }

    /// <summary>
    /// Serializes a generic event envelope to a JSON string.
    /// </summary>
    public string Serialize<T>(IEventEnvelope<T> eventEnvelope)
        where T : IMessage
    {
        return JsonConvert.SerializeObject(eventEnvelope, settings);
    }

    /// <summary>
    /// Deserializes a JSON string to an event envelope.
    /// </summary>
    public IEventEnvelope? Deserialize(string eventEnvelope, Type messageType)
    {
        // Get the generic type definition of EventEnvelope
        var eventEnvelopeType = typeof(EventEnvelope<>);
        var eventEnvelopGenericType = eventEnvelopeType.MakeGenericType(messageType);

        return JsonConvert.DeserializeObject(
            eventEnvelope,
            eventEnvelopGenericType,
            settings)
            as IEventEnvelope;
    }

    /// <summary>
    /// Deserializes a JSON string to a generic event envelope.
    /// </summary>
    public IEventEnvelope<T>? Deserialize<T>(string eventEnvelope)
        where T : IMessage
    {
        return JsonConvert.DeserializeObject<EventEnvelope<T>>(eventEnvelope, settings);
    }
}