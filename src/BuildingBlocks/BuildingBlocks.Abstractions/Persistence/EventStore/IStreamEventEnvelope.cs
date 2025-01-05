using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Abstractions.Persistence.EventStore;

/// <summary>
/// The Envelope Wrapper Pattern standardizes and enhances message handling by wrapping messages with metadata like
/// IDs, timestamps, and security tokens. Without the envelope, each consumer must independently manage and process
/// both the message and its metadata, leading to duplicated effort and complex logic within each consumer. With the
/// envelope, an initial envelope consumer processes the metadata (e.g., logging, validation) and extracts the core
/// message (in one place in the envelope object). It then forwards the message to dedicated business logic consumers,
/// simplifying their design and focusing them solely on payload processing, thereby improving maintainability and
/// scalability.
/// </summary>
/// Ref: https://www.enterpriseintegrationpatterns.com/patterns/messaging/EnvelopeWrapper.html
public interface IStreamEventEnvelope
{
    /// <summary>
    /// Gets the core event data.
    /// </summary>
    object Data { get; }

    /// <summary>
    /// Gets the metadata associated with the event.
    /// </summary>
    IStreamEventMetadata? Metadata { get; }
}

/// <summary>
/// Defines a wrapper for stream events with strongly-typed event data.
/// </summary>
/// <typeparam name="T">The type of the event data.</typeparam>
public interface IStreamEventEnvelope<out T> : IStreamEventEnvelope
    where T : IDomainEvent
{
    /// <summary>
    /// Gets the strongly-typed event data.
    /// </summary>
    new T Data { get; }
}