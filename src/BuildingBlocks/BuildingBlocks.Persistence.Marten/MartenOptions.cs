using Marten.Events.Daemon.Resiliency;

namespace BuildingBlocks.Persistence.Marten;

/// <summary>
/// Defines configuration options for Marten.
/// </summary>
public class MartenOptions
{
    private const string DefaultSchema = "public";

    /// <summary>
    /// Gets or sets the connection string for Marten.
    /// </summary>
    public string? ConnectionString { get; set; } = null;

    /// <summary>
    /// Gets or sets the schema for the write model. Defaults to "public".
    /// </summary>
    public string WriteModelSchema { get; set; } = DefaultSchema;

    /// <summary>
    /// Gets or sets the schema for the read model. Defaults to "public".
    /// </summary>
    public string ReadModelSchema { get; set; } = DefaultSchema;

    /// <summary>
    /// Gets or sets a value indicating whether to recreate the database.
    /// </summary>
    public bool ShouldRecreateDatabase { get; set; }

    /// <summary>
    /// Gets or sets the daemon mode for Marten. Defaults to "Disabled".
    /// </summary>
    public DaemonMode DaemonMode { get; set; } = DaemonMode.Disabled;

    /// <summary>
    /// Gets or sets a value indicating whether to use metadata in events.
    /// </summary>
    public bool UseMetadata { get; set; } = true;
}