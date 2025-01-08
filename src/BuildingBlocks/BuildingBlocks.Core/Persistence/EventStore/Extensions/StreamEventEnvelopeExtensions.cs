using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Persistence.EventStore;
using BuildingBlocks.Core.Reflection;

namespace BuildingBlocks.Core.Persistence.EventStore.Extensions;

/// <summary>
/// Provides extension methods for working with stream event envelopes.
/// </summary>
public static class StreamEventEnvelopeExtensions
{
    /// <summary>
    /// Converts a domain event to a stream event envelope.
    /// </summary>
    /// <param name="domainEvent">The domain event to convert.</param>
    /// <param name="metadata">The event metadata.</param>
    /// <returns>The stream event envelope.</returns>
    public static IStreamEventEnvelope ToStreamEvent(
        this IDomainEvent domainEvent,
        IStreamEventMetadata? metadata)
    {
        return ReflectionUtilities.CreateGenericType(
            typeof(StreamEventEnvelope<>),
            [domainEvent.GetType()],
            domainEvent,
            metadata
        );
    }
}