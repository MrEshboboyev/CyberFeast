using System.Text;
using BuildingBlocks.Abstractions.Persistence.EventStore;
using BuildingBlocks.Core.Persistence.EventStore;
using BuildingBlocks.Core.Types;
using EventStore.Client;
using Newtonsoft.Json;

namespace BuildingBlocks.Persistence.EventStoreDB.Extensions;

/// <summary>
/// Provides extension methods for serializing and deserializing data and metadata using EventStoreDB.
/// </summary>
public static class SerializationExtensions
{
    #region Deserialization

    /// <summary>
    /// Deserializes the data of a resolved event into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the data into.</typeparam>
    /// <param name="resolvedEvent">The resolved event.</param>
    /// <returns>The deserialized data.</returns>
    public static T DeserializeData<T>(this ResolvedEvent resolvedEvent) =>
        (T)DeserializeData(resolvedEvent);

    /// <summary>
    /// Deserializes the data of a resolved event into an object.
    /// </summary>
    /// <param name="resolvedEvent">The resolved event.</param>
    /// <returns>The deserialized data.</returns>
    public static object DeserializeData(this ResolvedEvent resolvedEvent)
    {
        // Get type
        var eventType = TypeMapper.GetType(resolvedEvent.Event.EventType);
        // Deserialize event
        return JsonConvert.DeserializeObject(
            Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span), eventType
        )!;
    }

    /// <summary>
    /// Deserializes the metadata of a resolved event into an instance of <see cref="IStreamEventMetadata"/>.
    /// </summary>
    /// <param name="resolvedEvent">The resolved event.</param>
    /// <returns>The deserialized metadata.</returns>
    public static IStreamEventMetadata DeserializeMetadata(this ResolvedEvent resolvedEvent)
    {
        // Deserialize event
        return JsonConvert.DeserializeObject<StreamEventMetadata>(
            Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.Span)
        )!;
    }

    #endregion

    #region Json Events

    /// <summary>
    /// Converts a stream event envelope to JSON event data.
    /// </summary>
    /// <param name="event">The stream event envelope.</param>
    /// <returns>The JSON event data.</returns>
    public static EventData ToJsonEventData(this IStreamEventEnvelope @event)
    {
        return ToJsonEventData(@event.Data, @event.Metadata);
    }

    /// <summary>
    /// Converts an object and metadata to JSON event data.
    /// </summary>
    /// <param name="event">The object to convert.</param>
    /// <param name="metadata">The metadata to include.</param>
    /// <returns>The JSON event data.</returns>
    public static EventData ToJsonEventData(this object @event, IStreamEventMetadata? metadata = null)
    {
        return new EventData(
            Uuid.NewUuid(),
            TypeMapper.GetTypeNameByObject(@event),
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(metadata ?? new object()))
        );
    }

    /// <summary>
    /// Converts a resolved event to a stream event envelope.
    /// </summary>
    /// <param name="resolvedEvent">The resolved event to convert.</param>
    /// <returns>The stream event envelope.</returns>
    public static IStreamEventEnvelope ToStreamEvent(this ResolvedEvent resolvedEvent)
    {
        var eventData = resolvedEvent.DeserializeData();
        var metaData = resolvedEvent.DeserializeMetadata();
        var type = typeof(StreamEventEnvelope<>).MakeGenericType(eventData.GetType());
        return (IStreamEventEnvelope)Activator.CreateInstance(type, eventData, metaData)!;
    }

    #endregion
}