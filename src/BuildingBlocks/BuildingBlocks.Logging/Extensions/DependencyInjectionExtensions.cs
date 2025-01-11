using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Extensions.ServiceCollection;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Settings.Configuration;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Sinks.Spectre;

namespace BuildingBlocks.Logging.Extensions;

/// <summary>
/// Provides extension methods for adding and configuring Serilog in a WebApplicationBuilder.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds and configures Serilog in the WebApplicationBuilder with options for console logging, ElasticSearch, Grafana Loki, Seq, and OpenTelemetry.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="extraConfigure">An optional action to perform additional configuration on the LoggerConfiguration.</param>
    /// <param name="configurator">An optional action to configure the SerilogOptions.</param>
    /// <returns>The updated web application builder.</returns>
    public static WebApplicationBuilder AddCustomSerilog(
        this WebApplicationBuilder builder,
        Action<LoggerConfiguration>? extraConfigure = null,
        Action<SerilogOptions>? configurator = null
    )
    {
        var serilogOptions = builder.Configuration.BindOptions<SerilogOptions>();
        configurator?.Invoke(serilogOptions);

        // Add options to the dependency injection
        builder.Services.AddValidationOptions<SerilogOptions>(opt => configurator?.Invoke(opt));

        // Serilog replaces ILoggerFactory, it replaces Microsoft's LoggerFactory class with SerilogLoggerFactory,
        // so ConsoleLoggerProvider and other default Microsoft logger providers don't get instantiated at all with Serilog
        builder.Host.UseSerilog((context, serviceProvider, loggerConfiguration) =>
        {
            extraConfigure?.Invoke(loggerConfiguration);

            loggerConfiguration
                .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
                // Segment enrichment options for further expansions
                // .Enrich.WithSpan()
                // .Enrich.WithBaggage()
                .Enrich.WithCorrelationIdHeader()
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .Enrich.WithExceptionDetails(
                    new DestructuringOptionsBuilder()
                        .WithDefaultDestructurers()
                        .WithDestructurers([new DbUpdateExceptionDestructurer()])
                );

            loggerConfiguration.ReadFrom.Configuration(
                context.Configuration,
                new ConfigurationReaderOptions { SectionName = nameof(SerilogOptions) }
            );

            if (serilogOptions.UseConsole)
            {
                loggerConfiguration.WriteTo.Async(writeTo =>
                    writeTo.Spectre(outputTemplate: serilogOptions.LogTemplate)
                );
            }

            if (!string.IsNullOrEmpty(serilogOptions.ElasticSearchUrl))
            {
                // Elasticsearch sink is inherently asynchronous
                loggerConfiguration.WriteTo.Elasticsearch(
                    [new Uri(serilogOptions.ElasticSearchUrl)],
                    opts =>
                    {
                        opts.DataStream = new DataStreamName(
                            $"{builder.Environment.ApplicationName}-{builder.Environment.EnvironmentName}-{DateTime.Now:yyyy-MM}"
                        );
                        opts.BootstrapMethod = BootstrapMethod.Failure;
                        opts.ConfigureChannel = channelOpts =>
                        {
                            channelOpts.BufferOptions = new BufferOptions
                            {
                                ExportMaxConcurrency = 10
                            };
                        };
                    }
                );
            }

            if (!string.IsNullOrEmpty(serilogOptions.GrafanaLokiUrl))
            {
                loggerConfiguration.WriteTo.GrafanaLoki(
                    serilogOptions.GrafanaLokiUrl,
                    [new LokiLabel
                    {
                        Key = "service",
                        Value = "food-delivery"
                    }],
                    ["app"]
                );
            }

            if (!string.IsNullOrEmpty(serilogOptions.SeqUrl))
            {
                // Seq sink is inherently asynchronous
                loggerConfiguration.WriteTo.Seq(serilogOptions.SeqUrl);
            }

            if (serilogOptions.ExportLogsToOpenTelemetry)
            {
                // Export logs from Serilog to OpenTelemetry
                loggerConfiguration.WriteTo.OpenTelemetry();
            }

            if (!string.IsNullOrEmpty(serilogOptions.LogPath))
            {
                loggerConfiguration.WriteTo.Async(writeTo =>
                    writeTo.File(
                        serilogOptions.LogPath,
                        outputTemplate: serilogOptions.LogTemplate,
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true
                    )
                );
            }
        });

        return builder;
    }
}