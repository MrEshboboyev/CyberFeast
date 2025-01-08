using System.Reflection;
using BuildingBlocks.Abstractions.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Persistence.Extensions;

/// <summary>
/// Provides extension methods for setting up dependency injection for persistence-related services.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds core persistence services to the service collection, including database executors and data seeders.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">The assemblies to scan for database executors and data seeders.</param>
    /// <returns>The updated service collection.</returns>
    internal static IServiceCollection AddPersistenceCore(
        this IServiceCollection services,
        params Assembly[] assembliesToScan)
    {
        services.ScanAndRegisterDbExecutors(assembliesToScan);

        services.RegisterDataSeeders(assembliesToScan);

        services.AddHostedService<SeedWorker>();
        services.AddScoped<IMigrationManager, MigrationManager>();

        return services;
    }

    /// <summary>
    /// Registers data seeders from the specified assemblies with the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">The assemblies to scan for data seeders.</param>
    public static void RegisterDataSeeders(
        this IServiceCollection services,
        Assembly[] assembliesToScan)
    {
        services.Scan(scan =>
            scan.FromAssemblies(assembliesToScan)
                .AddClasses(classes => classes.AssignableTo<IDataSeeder>())
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo<ITestDataSeeder>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());
    }
}