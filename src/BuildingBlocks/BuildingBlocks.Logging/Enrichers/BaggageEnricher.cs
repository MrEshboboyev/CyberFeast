using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace BuildingBlocks.Logging.Enrichers;

/// <summary>
/// Enriches logs with baggage information from the current <see cref="Activity"/>.
/// </summary>
public class BaggageEnricher : ILogEventEnricher
{
    /// <summary>
    /// Enriches the log event with baggage information.
    /// </summary>
    /// <param name="logEvent">The log event to enrich.</param>
    /// <param name="propertyFactory">The property factory to create log event properties.</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (Activity.Current == null)
            return;

        foreach (var (key, value) in Activity.Current.Baggage)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(key, value));
        }
    }
}