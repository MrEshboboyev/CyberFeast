using System.Reflection;
using BuildingBlocks.Abstractions.Web.Problem;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scrutor;

namespace BuildingBlocks.Web.Problem;

/// <summary>
/// Provides extension methods for registering custom problem details services and mappers.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds custom problem details services and mappers to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configure">Optional action to configure problem details options.</param>
    /// <param name="scanAssemblies">The assemblies to scan for mappers.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCustomProblemDetails(
        this IServiceCollection services,
        Action<ProblemDetailsOptions>? configure = null,
        params Assembly[] scanAssemblies)
    {
        var assemblies = scanAssemblies.Length != 0
            ? scanAssemblies
            : [Assembly.GetCallingAssembly()];

        services.AddProblemDetails(configure);
        services.Replace(ServiceDescriptor.Singleton<IProblemDetailsService, ProblemDetailsService>());

        RegisterAllMappers(services, assemblies);

        return services;
    }

    /// <summary>
    /// Registers all problem detail mappers from the specified assemblies in the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the mappers to.</param>
    /// <param name="scanAssemblies">The assemblies to scan for mappers.</param>
    private static void RegisterAllMappers(
        IServiceCollection services,
        Assembly[] scanAssemblies)
    {
        services.Scan(scan =>
            scan.FromAssemblies(scanAssemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IProblemDetailMapper)), false)
                .UsingRegistrationStrategy(RegistrationStrategy.Append)
                .As<IProblemDetailMapper>()
                .WithLifetime(ServiceLifetime.Singleton)
        );
    }
}