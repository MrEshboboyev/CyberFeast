using System.Reflection;
using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Persistence.EventStore;
using BuildingBlocks.Abstractions.Persistence.EventStore.Projections;
using BuildingBlocks.Core.Persistence.EventStore.InMemory;
using BuildingBlocks.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Core.Persistence.EventStore.Extensions;

/// <summary>
/// Provides extension methods for setting up dependency injection for event sourcing.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds an in-memory event store to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddInMemoryEventStore(this IServiceCollection services)
    {
        return AddEventSourcing<InMemoryEventStore>(services);
    }

    /// <summary>
    /// Adds event sourcing to the service collection with the specified event store type.
    /// </summary>
    /// <typeparam name="TEventStore">The type of the event store.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="scanAssemblies">The assemblies to scan for projections.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddEventSourcing<TEventStore>(
        this IServiceCollection services,
        params Assembly[] scanAssemblies)
        where TEventStore : class, IEventStore
    {
        // Assemblies are lazy loaded so using AppDomain.GetAssemblies is not reliable.
        var assemblies = scanAssemblies.Length != 0
            ? scanAssemblies
            : ReflectionUtilities.GetReferencedAssemblies(Assembly.GetCallingAssembly()).ToArray();

        services
            .AddSingleton<IAggregateStore, AggregateStore>()
            .AddSingleton<TEventStore, TEventStore>()
            .AddSingleton<IEventStore>(sp => sp.GetRequiredService<TEventStore>());

        services.AddReadProjections(assemblies);

        return services;
    }

    /// <summary>
    /// Adds read projections to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="scanAssemblies">The assemblies to scan for projections.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddReadProjections(
        this IServiceCollection services,
        params Assembly[] scanAssemblies)
    {
        services.TryAddSingleton<IReadProjectionPublisher, ReadProjectionPublisher>();
        RegisterProjections(services, scanAssemblies!);
        return services;
    }

    /// <summary>
    /// Registers projections in the specified assemblies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">The assemblies to scan for projections.</param>
    private static void RegisterProjections(
        IServiceCollection services,
        Assembly[] assembliesToScan)
    {
        services.Scan(scan => scan
            .FromAssemblies(assembliesToScan)
            .AddClasses(classes => classes.AssignableTo<IHaveReadProjection>()) // Filter classes
            .AsImplementedInterfaces()
            .WithTransientLifetime());
    }
}