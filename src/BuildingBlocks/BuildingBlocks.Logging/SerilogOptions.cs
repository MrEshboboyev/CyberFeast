namespace BuildingBlocks.Logging;

/// <summary>
/// Defines configuration options for Serilog logging.
/// </summary>
public sealed class SerilogOptions
{
    /// <summary>
    /// Gets or sets the URL for Seq logging.
    /// </summary>
    public string? SeqUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether console logging is enabled. Defaults to true.
    /// </summary>
    public bool UseConsole { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to export logs to OpenTelemetry.
    /// </summary>
    public bool ExportLogsToOpenTelemetry { get; set; }

    /// <summary>
    /// Gets or sets the URL for ElasticSearch.
    /// </summary>
    public string? ElasticSearchUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL for Grafana Loki.
    /// </summary>
    public string? GrafanaLokiUrl { get; set; }

    /// <summary>
    /// Gets or sets the log template for Serilog.
    /// </summary>
    public string LogTemplate { get; set; } =
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level} - {Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// Gets or sets the path for log files.
    /// </summary>
    public string? LogPath { get; set; }
}