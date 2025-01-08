namespace BuildingBlocks.Core.Events.Internal;

/// <summary>
/// Represents a notification event that contains arbitrary data.
/// </summary>
/// <param name="Data">The data associated with the notification event.</param>
public record NotificationEvent(dynamic Data) : DomainNotificationEvent;