namespace BuildingBlocks.Abstractions.Messages;

/// <summary>
/// Defines the structure of a message.
/// </summary>
public interface IMessage
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
