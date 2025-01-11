namespace BuildingBlocks.OpenTelemetry;

/// <summary>
/// Defines different types of tracing exporters.
/// </summary>
public enum TracingExporterType
{
    Console = 0,
    OTLP = 1,
    Jaeger = 2,
    Zipkin = 3,
    None = 4,
}

/// <summary>
/// Defines different types of metrics exporters.
/// </summary>
public enum MetricsExporterType
{
    Console = 0,
    OTLP = 1,
    None = 4,
}

/// <summary>
/// Defines different types of log exporters.
/// </summary>
public enum LogExporterType
{
    Console = 0,
    OTLP = 1,
    None = 4,
}