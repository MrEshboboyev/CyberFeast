namespace BuildingBlocks.Abstractions.Messaging.PersistMessage;

/// <summary>
/// Defines the possible delivery types of a message.
/// </summary>
[Flags]
public enum MessageDeliveryType
{
    /// <summary>
    /// Indicates that the message is for outbox delivery.
    /// </summary>
    Outbox = 1,

    /// <summary>
    /// Indicates that the message is for inbox delivery.
    /// </summary>
    Inbox = 2,

    /// <summary>
    /// Indicates that the message is for internal delivery.
    /// </summary>
    Internal = 4,
}