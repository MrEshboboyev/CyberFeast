using BuildingBlocks.Abstractions.Persistence.EventStore;
using EventStore.Client;

namespace BuildingBlocks.Persistence.EventStoreDB.Extensions;

/// <summary>
/// Provides extension methods for converting collections of <see cref="ResolvedEvent"/> to collections of <see cref="IStreamEventEnvelope"/>.
/// </summary>
public static class StreamEventExtensions
{
    /// <summary>
    /// Converts an enumerable collection of <see cref="ResolvedEvent"/> to an enumerable collection of <see cref="IStreamEventEnvelope"/>.
    /// </summary>
    /// <param name="resolvedEvents">The collection of resolved events to convert.</param>
    /// <returns>A collection of stream event envelopes.</returns>
    public static IEnumerable<IStreamEventEnvelope> ToStreamEvents(this IEnumerable<ResolvedEvent> resolvedEvents)
    {
        return resolvedEvents.Select(x => x.ToStreamEvent());
    }
}