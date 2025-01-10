using System.Reflection;
using BuildingBlocks.Abstractions.Core;
using BuildingBlocks.Abstractions.Types;
using BuildingBlocks.Core.Commands;
using BuildingBlocks.Core.Events.Extensions;
using BuildingBlocks.Core.Messaging.Extensions;
using BuildingBlocks.Core.Paging;
using BuildingBlocks.Core.Persistence.Extensions;
using BuildingBlocks.Core.Queries;
using BuildingBlocks.Core.Reflection;
using BuildingBlocks.Core.Serialization;
using BuildingBlocks.Core.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Sieve.Services;

namespace BuildingBlocks.Core.Extensions;

/// <summary>
/// Provides extension methods for registering core services with the dependency injection container.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds core services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="scanAssemblies">Assemblies to scan for services. Defaults to referenced assemblies of the calling assembly.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCore(
        this IServiceCollection services,
        params Assembly[] scanAssemblies)
    {
        var assemblies = scanAssemblies.Length != 0
            ? scanAssemblies
            : ReflectionUtilities.GetReferencedAssemblies(Assembly.GetCallingAssembly()).Distinct().ToArray();

        var systemInfo = MachineInstanceInfo.New();
        services.TryAddSingleton<IMachineInstanceInfo>(systemInfo);
        services.TryAddSingleton(systemInfo);
        services.TryAddSingleton<IExclusiveLock, ExclusiveLock>();

        services.TryAddScoped<ISieveProcessor, ApplicationSieveProcessor>();

        var policy = Policy.Handle<System.Exception>().RetryAsync(2);
        services.TryAddSingleton(policy);

        services.AddHttpContextAccessor();
        services.AddDefaultSerializer();
        services.AddMessagingCore(assemblies);
        services.AddCommandBus();
        services.AddQueryBus();
        services.AddEventBus(assemblies);
        services.AddPersistenceCore(assemblies);

        return services;
    }
}