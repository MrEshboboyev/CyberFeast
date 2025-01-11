using System.Diagnostics;
using BuildingBlocks.Logging.Enrichers;
using Serilog;
using Serilog.Configuration;

namespace BuildingBlocks.Logging;

/// <summary>
/// Provides extension methods for enriching logger output with baggage information from the current <see cref="Activity"/>.
/// </summary>
public static class LoggerEnrichmentConfigurationExtensions
{
    /// <summary>
    /// Enriches logger output with baggage information from the current <see cref="Activity"/>.
    /// </summary>
    /// <param name="loggerEnrichmentConfiguration">The enrichment configuration.</param>
    /// <returns>Configuration object allowing method chaining.</returns>
    public static LoggerConfiguration WithBaggage(this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration)
    {
        ArgumentNullException.ThrowIfNull(loggerEnrichmentConfiguration);
        return loggerEnrichmentConfiguration.With(new BaggageEnricher());
    }
}