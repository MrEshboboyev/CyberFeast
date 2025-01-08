using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Persistence.EventStore;

namespace BuildingBlocks.Core.Persistence.EventStore;

/// <summary>
/// Represents an envelope for wrapping event data and metadata.
/// </summary>
/// <typeparam name="T">The type of the event data.</typeparam>
public record StreamEventEnvelope<T>(
    T Data,
    IStreamEventMetadata? Metadata) : IStreamEventEnvelope<T>
    where T : IDomainEvent
{
    object IStreamEventEnvelope.Data => Data;
}

/// <summary>
/// Provides factory methods for creating stream event envelopes.
/// </summary>
public static class StreamEventEnvelope
{
    /// <summary>
    /// Creates a stream event envelope from the specified data and metadata.
    /// </summary>
    /// <param name="data">The event data.</param>
    /// <param name="metadata">The event metadata.</param>
    /// <returns>A stream event envelope containing the data and metadata.</returns>
    public static IStreamEventEnvelope From(
        object data,
        IStreamEventMetadata? metadata)
    {
        var type = typeof(StreamEventEnvelope<>).MakeGenericType(data.GetType());
        return (IStreamEventEnvelope)Activator.CreateInstance(type, data, metadata)!;
    }

    /// <summary>
    /// Creates a stream event envelope from the specified data and metadata.
    /// </summary>
    /// <typeparam name="TMessage">The type of the event data.</typeparam>
    /// <param name="data">The event data.</param>
    /// <param name="metadata">The event metadata.</param>
    /// <returns>A stream event envelope containing the data and metadata.</returns>
    public static IStreamEventEnvelope<TMessage> From<TMessage>(
        TMessage data,
        IStreamEventMetadata? metadata) where TMessage : IDomainEvent
    {
        return new StreamEventEnvelope<TMessage>(data, metadata);
    }

    /// <summary>
    /// Creates a stream event envelope from the specified data, event ID, stream position, and log position.
    /// </summary>
    /// <param name="data">The event data.</param>
    /// <param name="eventId">The event ID.</param>
    /// <param name="streamPosition">The stream position.</param>
    /// <param name="logPosition">The log position.</param>
    /// <returns>A stream event envelope containing the data and metadata.</returns>
    public static IStreamEventEnvelope From(
        object data,
        string eventId,
        ulong streamPosition,
        ulong logPosition)
    {
        var methodInfo = typeof(StreamEventEnvelope).GetMethods()
            .FirstOrDefault(x =>
                x.Name == nameof(From) &&
                x.GetGenericArguments().Length != 0 &&
                x.GetParameters().Length == 4);

        var genericMethod = methodInfo!.MakeGenericMethod(data.GetType());

        return (IStreamEventEnvelope)genericMethod
            .Invoke(null, [data, eventId, streamPosition, logPosition])!;
    }

    /// <summary>
    /// Creates a stream event envelope from the specified data, event ID, stream position, and log position.
    /// </summary>
    /// <typeparam name="T">The type of the event data.</typeparam>
    /// <param name="data">The event data.</param>
    /// <param name="eventId">The event ID.</param>
    /// <param name="streamPosition">The stream position.</param>
    /// <param name="logPosition">The log position.</param>
    /// <returns>A stream event envelope containing the data and metadata.</returns>
    public static IStreamEventEnvelope<T> From<T>(
        T data,
        string eventId,
        ulong streamPosition,
        ulong logPosition) where T : IDomainEvent
    {
        var envelopeMetadata = new StreamEventMetadata(eventId, streamPosition, logPosition);

        return From(data, envelopeMetadata);
    }
}