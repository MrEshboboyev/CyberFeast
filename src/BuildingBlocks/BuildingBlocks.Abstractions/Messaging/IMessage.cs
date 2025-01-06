using MediatR;

namespace BuildingBlocks.Abstractions.Messaging;

/// <summary>
/// Defines the structure of a message.
/// </summary>
public interface IMessage : INotification
{
    /// <summary>
    /// Gets the unique identifier of the message.
    /// </summary>
    Guid MessageId { get; }

    /// <summary>
    /// Gets the date and time when the message was created.
    /// </summary>
    DateTime Created { get; }
}