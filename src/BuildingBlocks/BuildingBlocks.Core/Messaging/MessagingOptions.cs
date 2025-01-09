namespace BuildingBlocks.Core.Messaging;

/// <summary>
/// Represents configuration options for messaging.
/// </summary>
public class MessagingOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the outbox feature is enabled.
    /// </summary>
    public bool OutboxEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the inbox feature is enabled.
    /// </summary>
    public bool InboxEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether endpoints should be automatically configured.
    /// </summary>
    public bool AutoConfigEndpoints { get; set; }
}