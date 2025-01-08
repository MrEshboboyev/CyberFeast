using BuildingBlocks.Abstractions.Messaging;
using MassTransit;

namespace BuildingBlocks.Core.Messaging;

/// <summary>
/// Represents a basic message with a unique identifier and a creation timestamp.
/// </summary>
public abstract record Message : IMessage
{
    /// <summary>
    /// Gets the unique identifier for the message.
    /// </summary>
    public Guid MessageId => NewId.NextGuid();

    /// <summary>
    /// Gets the timestamp when the message was created.
    /// </summary>
    public DateTime Created { get; } = DateTime.Now;
}