using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.Types;
using BuildingBlocks.Core.Types.Extensions;
using Humanizer;

namespace BuildingBlocks.Core.Events;

/// <summary>
/// Represents an envelope for wrapping events along with their metadata.
/// </summary>
/// <typeparam name="T">The type of the event message.</typeparam>
public record EventEnvelope<T>(T Message, EventEnvelopeMetadata Metadata) : IEventEnvelope<T>
    where T : IMessage
{
    object IEventEnvelope.Message => Message;
}

/// <summary>
/// Provides factory methods for creating event envelopes.
/// </summary>
public static class EventEnvelope
{
    /// <summary>
    /// Creates an event envelope from the specified data and metadata.
    /// </summary>
    /// <param name="data">The event data.</param>
    /// <param name="metadata">The event metadata.</param>
    /// <returns>An event envelope containing the data and metadata.</returns>
    public static IEventEnvelope From(object data, EventEnvelopeMetadata metadata)
    {
        var type = typeof(EventEnvelope<>).MakeGenericType(data.GetType());
        return (IEventEnvelope)Activator.CreateInstance(type, data, metadata)!;
    }

    /// <summary>
    /// Creates an event envelope from the specified data and metadata.
    /// </summary>
    /// <typeparam name="T">The type of the event message.</typeparam>
    /// <param name="data">The event data.</param>
    /// <param name="metadata">The event metadata.</param>
    /// <returns>An event envelope containing the data and metadata.</returns>
    public static IEventEnvelope<T> From<T>(T data, EventEnvelopeMetadata metadata)
        where T : IMessage
    {
        return new EventEnvelope<T>(data, metadata);
    }

    /// <summary>
    /// Creates an event envelope from the specified data, correlation ID, and optional caution ID and headers.
    /// </summary>
    /// <param name="data">The event data.</param>
    /// <param name="correlationId">The correlation ID.</param>
    /// <param name="cautionId">The optional caution ID.</param>
    /// <param name="headers">The optional headers.</param>
    /// <returns>An event envelope containing the data, correlation ID, caution ID, and headers.</returns>
    public static IEventEnvelope From(
        object data,
        Guid correlationId,
        Guid? cautionId = null,
        IDictionary<string, object?>? headers = null)
    {
        var methodInfo = typeof(EventEnvelope)
            .GetMethods()
            .FirstOrDefault(x =>
                x.Name == nameof(From) && x.GetGenericArguments().Length != 0 && x.GetParameters().Length == 4);
        var genericMethod = methodInfo.MakeGenericMethod(data.GetType());

        return (IEventEnvelope)genericMethod.Invoke(null, [data, correlationId, cautionId, headers]);
    }

    /// <summary>
    /// Creates an event envelope from the specified data, correlation ID, and optional caution ID and headers.
    /// </summary>
    /// <typeparam name="T">The type of the event message.</typeparam>
    /// <param name="data">The event data.</param>
    /// <param name="correlationId">The correlation ID.</param>
    /// <param name="cautionId">The optional caution ID.</param>
    /// <param name="headers">The optional headers.</param>
    /// <returns>An event envelope containing the data, correlation ID, caution ID, and headers.</returns>
    public static IEventEnvelope<T> From<T>(
        T data,
        Guid correlationId,
        Guid? cautionId = null,
        IDictionary<string, object?>? headers = null)
        where T : IMessage
    {
        var envelopeMetadata = new EventEnvelopeMetadata(
            data.MessageId,
            correlationId,
            TypeMapper.GetTypeName(data.GetType()),
            data.GetType().Name.Underscore(),
            cautionId)
        {
            CreatedUnixTime = DateTime.Now.ToUnixTimeSecond(),
            Headers = headers ?? new Dictionary<string, object?>()
        };

        return From(data, envelopeMetadata);
    }
}