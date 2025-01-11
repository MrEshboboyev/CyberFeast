namespace BuildingBlocks.Persistence.EventStoreDB;

/// <summary>
/// Defines the options for configuring EventStoreDB.
/// </summary>
public class EventStoreDbOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to use internal checkpointing. Defaults to true.
    /// </summary>
    public bool UseInternalCheckpointing { get; set; } = true;

    /// <summary>
    /// Gets or sets the host address for EventStoreDB.
    /// </summary>
    public string? Host { get; set; } = null;

    /// <summary>
    /// Gets or sets the HTTP port for EventStoreDB. Defaults to 2113.
    /// </summary>
    public int HttpPort { get; set; } = 2113;

    /// <summary>
    /// Gets or sets the TCP port for EventStoreDB. Defaults to 1113.
    /// </summary>
    public int TcpPort { get; set; } = 1113;

    /// <summary>
    /// Gets the gRPC connection string for EventStoreDB.
    /// </summary>
    public string GrpcConnectionString => $"esdb://{Host}:{HttpPort}?tls=false";

    /// <summary>
    /// Gets the TCP connection string for EventStoreDB.
    /// </summary>
    public string TcpConnectionString => $"tcp://{Host}:{TcpPort}?tls=false";

    /// <summary>
    /// Gets the HTTP connection string for EventStoreDB.
    /// </summary>
    public string HttpConnectionString => $"http://{Host}:{HttpPort}";

    /// <summary>
    /// Gets or sets the subscription options for EventStoreDB.
    /// </summary>
    public EventStoreDbSubscriptionOptions SubscriptionOptions { get; set; } = new EventStoreDbSubscriptionOptions();
}

/// <summary>
/// Defines the options for configuring EventStoreDB subscriptions.
/// </summary>
public class EventStoreDbSubscriptionOptions
{
    /// <summary>
    /// Gets or sets the subscription ID. Defaults to "default".
    /// </summary>
    public string SubscriptionId { get; set; } = "default";

    /// <summary>
    /// Gets or sets a value indicating whether to resolve link events to their linked events.
    /// </summary>
    public bool ResolveLinkTos { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore deserialization errors. Defaults to true.
    /// </summary>
    public bool IgnoreDeserializationErrors { get; set; } = true;
}