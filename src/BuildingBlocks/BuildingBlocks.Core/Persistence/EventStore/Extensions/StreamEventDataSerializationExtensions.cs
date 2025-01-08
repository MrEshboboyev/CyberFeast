using System.Text;
using BuildingBlocks.Abstractions.Persistence.EventStore;
using BuildingBlocks.Core.Types;
using Newtonsoft.Json;

namespace BuildingBlocks.Core.Persistence.EventStore.Extensions;

/// <summary>
/// Provides extension methods for serializing and deserializing event data.
/// </summary>
public static class StreamEventDataSerializationExtensions
{
    /// <summary>
    /// Deserializes event data to a specific type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="resolvedEvent">The resolved event data.</param>
    /// <returns>The deserialized event data.</returns>
    public static T DeserializeData<T>(this StreamEventData resolvedEvent) =>
        (T)DeserializeData(resolvedEvent);

    /// <summary>
    /// Deserializes event data.
    /// </summary>
    /// <param name="eventData">The event data to deserialize.</param>
    /// <returns>The deserialized event data.</returns>
    public static object DeserializeData(this StreamEventData eventData)
    {
        // Get type
        var eventType = TypeMapper.GetType(eventData.EventType);

        // Deserialize event
        return JsonConvert.DeserializeObject(
            Encoding.UTF8.GetString(eventData.Data),
            eventType)!;
    }

    /// <summary>
    /// Deserializes event metadata.
    /// </summary>
    /// <param name="eventData">The event data containing the metadata.</param>
    /// <returns>The deserialized event metadata.</returns>
    public static IStreamEventMetadata? DeserializeMetadata(this StreamEventData eventData)
    {
        return eventData.Metadata is null
            ? null
            : JsonConvert.DeserializeObject<StreamEventMetadata>(
                Encoding.UTF8.GetString(eventData.Metadata))!;
    }

    /// <summary>
    /// Converts event data to a stream event envelope.
    /// </summary>
    /// <param name="streamEventData">The stream event data.</param>
    /// <returns>The stream event envelope.</returns>
    public static IStreamEventEnvelope ToStreamEvent(this StreamEventData streamEventData)
    {
        var eventData = streamEventData.DeserializeData();
        var metadata = streamEventData.DeserializeMetadata();

        return StreamEventEnvelope.From(eventData, metadata);
    }

    /// <summary>
    /// Converts event data and metadata to JSON format.
    /// </summary>
    /// <param name="event">The event data to convert.</param>
    /// <returns>The JSON-formatted stream event data.</returns>
    public static StreamEventData ToJsonStreamEventData(this IStreamEventEnvelope @event)
    {
        return ToJsonStreamEventData(@event.Data, @event.Metadata);
    }

    /// <summary>
    /// Converts event data and metadata to JSON format.
    /// </summary>
    /// <param name="event">The event data to convert.</param>
    /// <param name="metadata">The event metadata.</param>
    /// <returns>The JSON-formatted stream event data.</returns>
    public static StreamEventData ToJsonStreamEventData(
        this object @event,
        IStreamEventMetadata? metadata = null)
    {
        return new StreamEventData
        {
            EventId = Guid.NewGuid(),
            EventType = TypeMapper.GetFullTypeNameByObject(@event),
            Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
            Metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(metadata ?? new object())),
            ContentType = "application/json"
        };
    }
}