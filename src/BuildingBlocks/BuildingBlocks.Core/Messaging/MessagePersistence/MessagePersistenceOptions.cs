namespace BuildingBlocks.Core.Messaging.MessagePersistence;

/// <summary>
/// Represents configuration options for message persistence.
/// </summary>
public class MessagePersistenceOptions
{
    /// <summary>
    /// Gets or sets the interval for processing messages.
    /// </summary>
    public int? Interval { get; set; } = 30;

    /// <summary>
    /// Gets or sets the connection string for the message persistence store.
    /// </summary>
    public string? ConnectionString { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether message persistence is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the assembly containing migration classes.
    /// </summary>
    public string? MigrationAssembly { get; set; } = null;
}