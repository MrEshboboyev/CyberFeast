using System.Diagnostics.CodeAnalysis;
using BuildingBlocks.Abstractions.Domain.EventSourcing;
using BuildingBlocks.Core.Extensions;

namespace BuildingBlocks.Core.Persistence.EventStore;

/// <summary>
/// Represents the name of a stream in the event store.
/// </summary>
public class StreamName
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamName"/> class.
    /// </summary>
    /// <param name="value">The value of the stream name.</param>
    public StreamName([NotNull] string? value)
    {
        Value = value.NotBeNull();
    }

    /// <summary>
    /// Gets the value of the stream name.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a stream name for a given aggregate type and ID.
    /// </summary>
    /// <typeparam name="T">The type of the aggregate.</typeparam>
    /// <param name="id">The ID of the aggregate.</param>
    /// <returns>A new instance of <see cref="StreamName"/> for the aggregate type and ID.</returns>
    public static StreamName For<T>(string id) =>
        new($"{typeof(T).Name}-{id.NotBeNullOrWhiteSpace()}");

    /// <summary>
    /// Creates a stream name for a given aggregate type and ID.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TId">The type of the aggregate ID.</typeparam>
    /// <param name="aggregateId">The ID of the aggregate.</param>
    /// <returns>A new instance of <see cref="StreamName"/> for the aggregate type and ID.</returns>
    public static StreamName For<TAggregate, TId>(TId aggregateId)
        where TAggregate : IEventSourcedAggregate<TId>
    {
        aggregateId.NotBeNull();

        var id = aggregateId
            .ToString()
            .NotBeNullOrWhiteSpace();

        return For<TAggregate>(id);
    }

    /// <summary>
    /// Implicitly converts a <see cref="StreamName"/> to a string.
    /// </summary>
    /// <param name="streamName">The stream name to convert.</param>
    /// <returns>The string representation of the stream name.</returns>
    public static implicit operator string(StreamName streamName) => streamName.Value;

    /// <summary>
    /// Returns the string representation of the stream name.
    /// </summary>
    /// <returns>The string representation of the stream name.</returns>
    public override string ToString() => Value;
}