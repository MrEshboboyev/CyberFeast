using System.Reflection;
using System.Runtime.InteropServices;
using BuildingBlocks.Abstractions.Web.Module;
using BuildingBlocks.Core.Reflection;
using BuildingBlocks.Core.Reflection.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web.Modules;

/// <summary>
/// Provides methods for adding and configuring services and endpoints for modular applications.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds module services to the <see cref="WebApplicationBuilder"/>.
    /// </summary>
    /// <param name="webApplicationBuilder">The web application builder.</param>
    /// <param name="scanAssemblies">The assemblies to scan for modules.</param>
    /// <returns>The updated web application builder.</returns>
    public static WebApplicationBuilder AddModulesServices(
        this WebApplicationBuilder webApplicationBuilder,
        params Assembly[] scanAssemblies)
    {
        var assemblies = LoadAssemblies(scanAssemblies);

        var modulesConfiguration = assemblies
            .SelectMany(x => x.GetLoadableTypes())
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType &&
                        !t.IsInterface && t.GetConstructor(Type.EmptyTypes) != null &&
                        typeof(IModuleConfiguration).IsAssignableFrom(t))
            .ToList();

        var sharedModulesConfiguration = assemblies
            .SelectMany(x => x.GetLoadableTypes())
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType &&
                        !t.IsInterface && t.GetConstructor(Type.EmptyTypes) != null &&
                        typeof(ISharedModulesConfiguration).IsAssignableFrom(t))
            .ToList();

        foreach (var sharedModule in CollectionsMarshal.AsSpan(sharedModulesConfiguration))
        {
            AddModulesDependencyInjection(webApplicationBuilder, sharedModule);
        }

        foreach (var module in CollectionsMarshal.AsSpan(modulesConfiguration))
        {
            AddModulesDependencyInjection(webApplicationBuilder, module);
        }

        return webApplicationBuilder;
    }

    /// <summary>
    /// Configures modules in the web application.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The updated web application.</returns>
    public static async Task<WebApplication> ConfigureModules(this WebApplication app)
    {
        var moduleConfigurations = app.Services.GetServices<IModuleConfiguration>();
        var sharedModulesConfigurations = app.Services.GetServices<ISharedModulesConfiguration>();

        foreach (var sharedModule in sharedModulesConfigurations)
        {
            await sharedModule.ConfigureSharedModule(app);
        }

        foreach (var module in moduleConfigurations)
        {
            await module.ConfigureModule(app);
        }

        return app;
    }

    /// <summary>
    /// Maps the endpoints for all registered modules.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <returns>The updated endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapModulesEndpoints(this IEndpointRouteBuilder builder)
    {
        var modules = builder.ServiceProvider.GetServices<IModuleConfiguration>();
        var sharedModules = builder.ServiceProvider.GetServices<ISharedModulesConfiguration>();

        foreach (var module in sharedModules)
        {
            module.MapSharedModuleEndpoints(builder);
        }

        foreach (var module in modules)
        {
            module.MapEndpoints(builder);
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

    /// <summary>
    /// Registers module dependencies in the service collection.
    /// </summary>
    /// <param name="webApplicationBuilder">The web application builder.</param>
    /// <param name="module">The module type to register.</param>
    private static void AddModulesDependencyInjection(WebApplicationBuilder webApplicationBuilder, Type module)
    {
        if (module.IsAssignableTo(typeof(IModuleConfiguration)))
        {
            var instantiatedType = (IModuleConfiguration)Activator.CreateInstance(module)!;
            instantiatedType.AddModuleServices(webApplicationBuilder);
            webApplicationBuilder.Services.AddSingleton(instantiatedType);
        }

        if (module.IsAssignableTo(typeof(ISharedModulesConfiguration)))
        {
            var instantiatedType = (ISharedModulesConfiguration)Activator.CreateInstance(module)!;
            instantiatedType.AddSharedModuleServices(webApplicationBuilder);
            webApplicationBuilder.Services.AddSingleton(instantiatedType);
        }
    }
}