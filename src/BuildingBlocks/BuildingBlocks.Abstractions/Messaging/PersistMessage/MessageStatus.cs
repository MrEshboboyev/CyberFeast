namespace BuildingBlocks.Abstractions.Messaging.PersistMessage;

/// <summary>
/// Defines the possible statuses of a message.
/// </summary>
public enum MessageStatus
{
    /// <summary>
    /// Indicates that the message is stored.
    /// </summary>
    Stored = 1,

    /// <summary>
    /// Indicates that the message has been processed.
    /// </summary>
    Processed = 2,
}