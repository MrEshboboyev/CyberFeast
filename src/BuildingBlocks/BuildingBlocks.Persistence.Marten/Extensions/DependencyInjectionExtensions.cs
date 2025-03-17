using System.Reflection;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Extensions.ServiceCollection;
using BuildingBlocks.Core.Persistence.EventStore.Extensions;
using BuildingBlocks.Persistence.Marten.Subscriptions;
using JasperFx;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Weasel.Core;

namespace BuildingBlocks.Persistence.Marten.Extensions;

/// <summary>
/// Provides extension methods for configuring and adding Marten services to the dependency injection container.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Configures and adds Marten services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configureOptions">An optional action to configure StoreOptions.</param>
    /// <param name="configurator">An optional action to configure MartenOptions.</param>
    /// <param name="scanAssemblies">Assemblies to scan for event sourcing.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMartenDb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<StoreOptions>? configureOptions = null,
        Action<MartenOptions>? configurator = null,
        params Assembly[] scanAssemblies
    )
    {
        var assembliesToScan = scanAssemblies.Length != 0 
            ? scanAssemblies 
            : [Assembly.GetCallingAssembly()];

        var martenOptions = configuration.BindOptions<MartenOptions>();
        configurator?.Invoke(martenOptions);

        // Add options to the dependency injection
        services.AddValidationOptions<MartenOptions>(opt => configurator?.Invoke(opt));

        services.AddEventSourcing<MartenEventStore>(assembliesToScan);

        services.AddMarten(sp =>
            {
                var storeOptions = new StoreOptions();
                storeOptions.Connection(martenOptions.ConnectionString);
                storeOptions.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;

                var schemaName = Environment.GetEnvironmentVariable("SchemaName");
                storeOptions.Events.DatabaseSchemaName = schemaName ?? martenOptions.WriteModelSchema;
                storeOptions.DatabaseSchemaName = schemaName ?? martenOptions.ReadModelSchema;

                storeOptions.UseNewtonsoftForSerialization(
                    EnumStorage.AsString,
                    nonPublicMembersStorage: NonPublicMembersStorage.All
                );

                storeOptions.Projections.Add(
                    new MartenSubscription(
                        [new MartenEventPublisher(sp.GetRequiredService<IMediator>())],
                        sp.GetRequiredService<ILogger<MartenSubscription>>()
                    ),
                    ProjectionLifecycle.Async,
                    "MartenSubscription"
                );

                if (martenOptions.UseMetadata)
                {
                    storeOptions.Events.MetadataConfig.CausationIdEnabled = true;
                    storeOptions.Events.MetadataConfig.CorrelationIdEnabled = true;
                    storeOptions.Events.MetadataConfig.HeadersEnabled = true;
                }

                configureOptions?.Invoke(storeOptions);

                return storeOptions;
            })
            .UseLightweightSessions()
            .ApplyAllDatabaseChangesOnStartup()
            .AddAsyncDaemon(DaemonMode.Solo);

        return services;
    }
}
