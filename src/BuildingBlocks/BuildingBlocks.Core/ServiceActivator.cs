using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core;

/// <summary>
/// Provides static methods to interact with the IServiceProvider.
/// </summary>
public static class ServiceActivator
{
    /// <summary>
    /// The service provider instance.
    /// </summary>
    internal static IServiceProvider? _serviceProvider;

    /// <summary>
    /// Configures the service activator with the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider to configure.</param>
    public static void Configure(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Creates a new service scope using the specified or configured service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider to use. If null, uses the configured service provider.</param>
    /// <returns>A new service scope.</returns>
    public static IServiceScope? GetScope(IServiceProvider? serviceProvider = null)
    {
        var provider = serviceProvider ?? _serviceProvider;
        return provider?.GetRequiredService<IServiceScopeFactory>().CreateScope();
    }

    /// <summary>
    /// Gets a service of the specified type from the configured service provider.
    /// </summary>
    /// <typeparam name="T">The type of the service to retrieve.</typeparam>
    /// <returns>An instance of the specified service type.</returns>
    public static T? GetService<T>()
    {
        return _serviceProvider.GetService<T>();
    }

    /// <summary>
    /// Gets a required service of the specified type from the configured service provider.
    /// </summary>
    /// <typeparam name="T">The type of the service to retrieve.</typeparam>
    /// <returns>An instance of the specified service type.</returns>
    public static T GetRequiredService<T>()
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Gets a required service of the specified type from the configured service provider.
    /// </summary>
    /// <param name="type">The type of the service to retrieve.</param>
    /// <returns>An instance of the specified service type.</returns>
    public static object GetRequiredServices(Type type)
    {
        return _serviceProvider.GetRequiredService(type);
    }

    /// <summary>
    /// Gets all services of the specified type from the configured service provider.
    /// </summary>
    /// <typeparam name="T">The type of the services to retrieve.</typeparam>
    /// <returns>An enumerable collection of the specified service type.</returns>
    public static IEnumerable<T> GetServices<T>()
    {
        return _serviceProvider.GetServices<T>();
    }

    /// <summary>
    /// Gets a service of the specified type from the configured service provider.
    /// </summary>
    /// <param name="type">The type of the service to retrieve.</param>
    /// <returns>An instance of the specified service type.</returns>
    public static object GetService(Type type)
    {
        return _serviceProvider.GetService(type);
    }

    /// <summary>
    /// Gets all services of the specified type from the configured service provider.
    /// </summary>
    /// <param name="type">The type of the services to retrieve.</param>
    /// <returns>An enumerable collection of the specified service type.</returns>
    public static IEnumerable<object> GetServices(Type type)
    {
        return _serviceProvider.GetServices(type);
    }
}