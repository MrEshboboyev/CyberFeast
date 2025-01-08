using System.Reflection;
using System.Runtime.InteropServices;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Core.Reflection.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Persistence.Extensions;

/// <summary>
/// Provides extension methods for scanning and registering database service registrars.
/// </summary>
internal static class DbExecutorRegistrationsExtensions
{
    /// <summary>
    /// Scans the specified assemblies for types that implement <see cref="IDbServiceRegistrar"/> and registers them with the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">The assemblies to scan for database service registrars.</param>
    /// <returns>The updated service collection.</returns>
    internal static IServiceCollection ScanAndRegisterDbExecutors(
        this IServiceCollection services,
        params Assembly[] assembliesToScan)
    {
        var dbExecutors = assembliesToScan
            .SelectMany(x => x.GetLoadableTypes())
            .Where(t =>
                t!.IsClass
                && t is
                {
                    IsAbstract: false,
                    IsGenericType: false,
                    IsInterface: false
                }
                && t.GetConstructor(Type.EmptyTypes) != null
                && typeof(IDbServiceRegistrar).IsAssignableFrom(t))
            .ToList();

        foreach (var dbExecutor in CollectionsMarshal.AsSpan(dbExecutors))
        {
            var instantiatedType = (IDbServiceRegistrar)Activator.CreateInstance(dbExecutor!)!;
            instantiatedType.Register(services);
        }

        return services;
    }
}