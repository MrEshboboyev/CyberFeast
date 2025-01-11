namespace BuildingBlocks.OpenTelemetry;

/// <summary>
/// Configures OpenTelemetry options, including exporter types and specific exporter options.
/// </summary>
public class OpenTelemetryOptions
{
    /// <summary>
    /// Gets or sets the type of tracing exporter. Defaults to "None".
    /// </summary>
    public string TracingExporterType { get; set; } = nameof(OpenTelemetry.TracingExporterType.None);

    /// <summary>
    /// Gets or sets the type of log exporter. Defaults to "None".
    /// </summary>
    public string LogExporterType { get; set; } = nameof(OpenTelemetry.LogExporterType.None);

    /// <summary>
    /// Gets or sets the type of metrics exporter. Defaults to "None".
    /// </summary>
    public string MetricsExporterType { get; set; } = nameof(OpenTelemetry.MetricsExporterType.None);

    /// <summary>
    /// Gets or sets the options for Jaeger exporter.
    /// </summary>
    public JaegerExporterOptions JaegerOptions { get; set; } = null!;

    /// <summary>
    /// Gets or sets the options for Zipkin exporter.
    /// </summary>
    public ZipkinExporterOptions ZipkinOptions { get; set; } = null!;

    /// <summary>
    /// Gets or sets the options for OTLP exporter.
    /// </summary>
    public OtlpExporterOptions OtlpOptions { get; set; } = null!;
}

/// <summary>
/// Configures options for Jaeger exporter.
/// </summary>
public class JaegerExporterOptions
{
    /// <summary>
    /// Gets or sets the Jaeger agent host.
    /// </summary>
    public string AgentHost { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Jaeger agent port.
    /// </summary>
    public int AgentPort { get; set; }
}

/// <summary>
/// Configures options for Zipkin exporter.
/// </summary>
public class ZipkinExporterOptions
{
    /// <summary>
    /// Gets or sets the Zipkin endpoint.
    /// </summary>
    public string Endpoint { get; set; } = null!;
}

/// <summary>
/// Configures options for OTLP exporter.
/// </summary>
public class OtlpExporterOptions
{
    /// <summary>
    /// Gets or sets the OTLP endpoint.
    /// </summary>
    public string OTLPEndpoint { get; set; } = null!;
}