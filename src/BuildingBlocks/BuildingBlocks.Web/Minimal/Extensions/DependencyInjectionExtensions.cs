using System.Reflection;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using BuildingBlocks.Core.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace BuildingBlocks.Web.Minimal.Extensions;

/// <summary>
/// Provides extension methods for registering minimal endpoints and mapping them in the endpoint routing.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers minimal endpoints from the specified assemblies in the service collection.
    /// </summary>
    /// <param name="applicationBuilder">The web application builder.</param>
    /// <param name="scanAssemblies">The assemblies to scan for endpoints.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMinimalEndpoints(
        this WebApplicationBuilder applicationBuilder,
        params Assembly[] scanAssemblies)
    {
        var assemblies = LoadAssemblies(scanAssemblies);

        applicationBuilder.Services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo<IMinimalEndpoint>())
                .UsingRegistrationStrategy(RegistrationStrategy.Append)
                .As<IMinimalEndpoint>()
                .WithLifetime(ServiceLifetime.Scoped)
        );

        return applicationBuilder.Services;
    }

    /// <summary>
    /// Registers minimal endpoints from the specified assemblies in the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="scanAssemblies">The assemblies to scan for endpoints.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMinimalEndpoints(
        this IServiceCollection services,
        params Assembly[] scanAssemblies)
    {
        var assemblies = LoadAssemblies(scanAssemblies);

        services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo<IMinimalEndpoint>())
                .UsingRegistrationStrategy(RegistrationStrategy.Append)
                .As<IMinimalEndpoint>()
                .WithLifetime(ServiceLifetime.Scoped)
        );

        return services;
    }

    /// <summary>
    /// Maps the registered minimal endpoints in the endpoint routing.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <returns>The updated endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapMinimalEndpoints(this IEndpointRouteBuilder builder)
    {
        var scope = builder.ServiceProvider.CreateScope();

        var endpoints = scope.ServiceProvider.GetServices<IMinimalEndpoint>().ToList();

        var versionGroups = endpoints
            .GroupBy(x => x.GroupName)
            .ToDictionary(x => x.Key, c => builder.NewVersionedApi(c.Key).WithTags(c.Key));

        var versionSubGroups = endpoints
            .GroupBy(x => new { x.GroupName, x.PrefixRoute, x.Version })
            .ToDictionary(
                x => x.Key,
                c => versionGroups[c.Key.GroupName].MapGroup(c.Key.PrefixRoute).HasApiVersion(c.Key.Version)
            );

        var endpointVersions = endpoints
            .GroupBy(x => new { x.GroupName, x.Version })
            .Select(x => new { x.Key.Version, x.Key.GroupName, Endpoints = x.Select(v => v) });

        foreach (var endpointVersion in endpointVersions)
        {
            var versionGroup = versionSubGroups
                .FirstOrDefault(x => x.Key.GroupName == endpointVersion.GroupName)
                .Value;

            foreach (var ep in endpointVersion.Endpoints)
            {
                ep.MapEndpoint(versionGroup);
            }
        }

        return builder;
    }

    /// <summary>
    /// Loads the specified assemblies or the calling assembly if no assemblies are specified.
    /// </summary>
    /// <param name="scanAssemblies">The assemblies to scan.</param>
    /// <returns>An array of assemblies to scan.</returns>
    private static Assembly[] LoadAssemblies(Assembly[] scanAssemblies)
    {
        return scanAssemblies.Length != 0
            ? scanAssemblies
            : ReflectionUtilities.GetReferencedAssemblies(Assembly.GetCallingAssembly())
                .Concat(ReflectionUtilities.GetApplicationPartAssemblies(Assembly.GetCallingAssembly()))
                .Distinct()
                .ToArray();
    }
}