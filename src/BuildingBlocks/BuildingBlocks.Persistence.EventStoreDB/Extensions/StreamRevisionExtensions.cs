using BuildingBlocks.Abstractions.Persistence.EventStore;
using EventStore.Client;

namespace BuildingBlocks.Persistence.EventStoreDB.Extensions;

/// <summary>
/// Provides extension methods for converting custom stream position and version types to <see cref="StreamRevision"/> and <see cref="StreamPosition"/> types.
/// </summary>
public static class StreamRevisionExtensions
{
    /// <summary>
    /// Converts a custom stream version to a <see cref="StreamRevision"/> type.
    /// </summary>
    /// <param name="version">The custom stream version.</param>
    /// <returns>The corresponding <see cref="StreamRevision"/> type.</returns>
    public static StreamRevision AsStreamRevision(this ExpectedStreamVersion version) =>
        StreamRevision.FromInt64(version.Value);

    /// <summary>
    /// Converts a custom stream truncate position to a <see cref="StreamPosition"/> type.
    /// </summary>
    /// <param name="position">The custom stream truncate position.</param>
    /// <returns>The corresponding <see cref="StreamPosition"/> type.</returns>
    public static StreamPosition AsStreamPosition(this StreamTruncatePosition position) =>
        StreamPosition.FromInt64(position.Value);

    /// <summary>
    /// Converts a custom stream read position to a <see cref="StreamPosition"/> type.
    /// </summary>
    /// <param name="position">The custom stream read position.</param>
    /// <returns>The corresponding <see cref="StreamPosition"/> type.</returns>
    public static StreamPosition AsStreamPosition(this StreamReadPosition position) =>
        StreamPosition.FromInt64(position.Value);
}