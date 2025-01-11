using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Extensions.ServiceCollection;
using BuildingBlocks.Core.Persistence.EventStore.Extensions;
using BuildingBlocks.Persistence.EventStoreDB.Subscriptions;
using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Persistence.EventStoreDB.Extensions;

/// <summary>
/// Provides extension methods for configuring and adding EventStoreDB services to the dependency injection container.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds and configures EventStoreDB services with specified options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="configurator">An optional action to configure the EventStoreDbOptions.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddEventStoreDb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<EventStoreDbOptions>? configurator = null
    )
    {
        var options = configuration.BindOptions<EventStoreDbOptions>();
        configurator?.Invoke(options);

        // Add options to the dependency injection
        services.AddValidationOptions<EventStoreDbOptions>(opt => configurator?.Invoke(opt));

        services.TryAddSingleton(new EventStoreClient(EventStoreClientSettings.Create(options.GrpcConnectionString)));

        services.AddEventSourcing<EventStoreDbEventStore>();

        if (options.UseInternalCheckpointing)
        {
            services.TryAddTransient<ISubscriptionCheckpointRepository, EventStoreDbSubscriptionCheckPointRepository>();
        }

        return services;
    }

    /// <summary>
    /// Adds hosted service for subscribing to all EventStoreDB events with optional checkpointing.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="checkpointToEventStoreDb">Indicates whether to store checkpoints in EventStoreDB.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddEventStoreDbSubscriptionToAll(
        this IServiceCollection services,
        bool checkpointToEventStoreDb = true
    )
    {
        if (checkpointToEventStoreDb)
        {
            services.TryAddTransient<ISubscriptionCheckpointRepository, EventStoreDbSubscriptionCheckPointRepository>();
        }

        return services.AddHostedService<EventStoreDbSubscriptionToAll>();
    }
}