using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Core.Extensions.ServiceCollection;

/// <summary>
/// Provides extension methods for the IServiceCollection interface, allowing more flexible service registration.
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a transient service of the type specified in serviceType with an implementation 
    /// type specified in implementationType to the IServiceCollection if the exact pairing 
    /// does not already exist.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <param name="implementationType">The type of the implementation to use.</param>
    public static void TryAddTransientExact(
        this IServiceCollection services,
        Type serviceType,
        Type implementationType
    )
    {
        if (services.Any(reg => reg.ServiceType == serviceType &&
                                reg.ImplementationType == implementationType))
        {
            return;
        }

        services.TryAddTransient(serviceType, implementationType);
    }

    /// <summary>
    /// Adds a scoped service of the type specified in serviceType with an implementation 
    /// type specified in implementationType to the IServiceCollection if the exact pairing 
    /// does not already exist.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <param name="implementationType">The type of the implementation to use.</param>
    public static void TryAddScopeExact(
        this IServiceCollection services,
        Type serviceType,
        Type implementationType
    )
    {
        if (services.Any(reg => reg.ServiceType == serviceType
                                && reg.ImplementationType == implementationType))
        {
            return;
        }

        services.TryAddScoped(serviceType, implementationType);
    }

    /// <summary>
    /// Adds a service of the type specified in TService with an implementation 
    /// type specified in TImplementation to the IServiceCollection using the specified 
    /// service lifetime.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="implementationFactory">The factory method that creates the service.</param>
    /// <param name="serviceLifetime">The lifetime of the service. Default is transient.</param>
    public static IServiceCollection Add<TService, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient
    )
        where TService : class
        where TImplementation : class, TService
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                services.TryAddSingleton(implementationFactory);
                return services;

            case ServiceLifetime.Scoped:
                services.TryAddScoped(implementationFactory);
                return services;

            case ServiceLifetime.Transient:
                services.TryAddTransient(implementationFactory);
                return services;

            default:
                throw new ArgumentNullException(nameof(serviceLifetime));
        }
    }

    /// <summary>
    /// Adds a service of the type specified in TService with an implementation 
    /// factory to the IServiceCollection using the specified service lifetime.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="implementationFactory">The factory method that creates the service.</param>
    /// <param name="serviceLifetime">The lifetime of the service. Default is transient.</param>
    public static IServiceCollection Add<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient
    )
        where TService : class
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                services.TryAddSingleton(implementationFactory);
                return services;

            case ServiceLifetime.Scoped:
                services.TryAddScoped(implementationFactory);
                return services;

            case ServiceLifetime.Transient:
                services.TryAddTransient(implementationFactory);
                return services;

            default:
                throw new ArgumentNullException(nameof(serviceLifetime));
        }
    }

    /// <summary>
    /// Adds a service of the type specified in TService to the 
    /// IServiceCollection using the specified service lifetime.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceLifetime">The lifetime of the service. Default is transient.</param>
    public static IServiceCollection Add<TService>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient
    )
        where TService : class
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                services.TryAddSingleton<TService>();
                return services;

            case ServiceLifetime.Scoped:
                services.TryAddScoped<TService>();
                return services;

            case ServiceLifetime.Transient:
                services.TryAddTransient<TService>();
                return services;

            default:
                throw new ArgumentNullException(nameof(serviceLifetime));
        }
    }

    /// <summary>
    /// Adds a service of the specified type to the IServiceCollection using 
    /// the specified service lifetime.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <param name="serviceLifetime">The lifetime of the service. Default is transient.</param>
    public static IServiceCollection Add(
        this IServiceCollection services,
        Type serviceType,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient
    )
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                services.TryAddSingleton(serviceType);
                return services;

            case ServiceLifetime.Scoped:
                services.TryAddScoped(serviceType);
                return services;

            case ServiceLifetime.Transient:
                services.TryAddTransient(serviceType);
                return services;

            default:
                throw new ArgumentNullException(nameof(serviceLifetime));
        }
    }

    /// <summary>
    /// Adds a service of the type specified in TService with an implementation 
    /// type specified in TImplementation to the IServiceCollection using the specified 
    /// service lifetime.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceLifetime">The lifetime of the service. Default is transient.</param>
    public static IServiceCollection Add<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient
    )
        where TService : class
        where TImplementation : class, TService
    {
        return serviceLifetime switch
        {
            ServiceLifetime.Singleton => services.AddSingleton<TService, TImplementation>(),
            ServiceLifetime.Scoped => services.AddScoped<TService, TImplementation>(),
            ServiceLifetime.Transient => services.AddTransient<TService, TImplementation>(),
            _ => throw new ArgumentNullException(nameof(serviceLifetime))
        };
    }

    /// <summary>
    /// Adds a service of the specified type with an implementation 
    /// factory to the IServiceCollection using the specified service lifetime.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <param name="implementationFactory">The factory method that creates the service.</param>
    /// <param name="serviceLifetime">The lifetime of the service. Default is transient.</param>
    public static IServiceCollection Add(
        this IServiceCollection services,
        Type serviceType,
        Func<IServiceProvider, object> implementationFactory,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient
    )
    {
        return serviceLifetime switch
        {
            ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationFactory),
            ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationFactory),
            ServiceLifetime.Transient => services.AddTransient(serviceType, implementationFactory),
            _ => throw new ArgumentNullException(nameof(serviceLifetime))
        };
    }

    /// <summary>
    /// Adds a service of the specified type with an implementation 
    /// type to the IServiceCollection using the specified service lifetime.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <param name="implementationType">The implementation type of the service to add.</param>
    /// <param name="serviceLifetime">The lifetime of the service. Default is transient.</param>
    public static IServiceCollection Add(
        this IServiceCollection services,
        Type serviceType,
        Type implementationType,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient
    )
    {
        return serviceLifetime switch
        {
            ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
            ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
            ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
            _ => throw new ArgumentNullException(nameof(serviceLifetime))
        };
    }

    /// <summary>
    /// Validates the dependencies registered in the IServiceCollection to ensure 
    /// they can be resolved successfully from the root service provider.
    /// </summary>
    /// <param name="rootServiceProvider">The root service provider.</param>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">Assemblies to scan for dependencies.</param>
    public static void ValidateDependencies(
        this IServiceProvider rootServiceProvider,
        IServiceCollection services,
        params Assembly[] assembliesToScan
    )
    {
        var scanAssemblies = assembliesToScan.Length != 0
            ? assembliesToScan
            : new Assembly[] { Assembly.GetExecutingAssembly() };

        // Create a scope to resolve scoped dependencies without errors
        using var scope = rootServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        foreach (var serviceDescriptor in services)
        {
            // Skip services that are not typically resolved directly or are special cases
            if (serviceDescriptor.ServiceType == typeof(IHostedService)
                || serviceDescriptor.ServiceType == typeof(IApplicationLifetime))
            {
                continue;
            }

            try
            {
                var serviceType = serviceDescriptor.ServiceType;
                if (scanAssemblies.Contains(serviceType.Assembly))
                {
                    // Attempt to resolve the service
                    var service = sp.GetService(serviceType);

                    // Assert: Check that the service was resolved if it's not meant to be optional
                    if (serviceDescriptor.ImplementationInstance == null
                        && serviceDescriptor.ImplementationFactory == null)
                    {
                        service.NotBeNull();
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(
                    $"Failed to resolve service {serviceDescriptor.ServiceType.FullName}: {ex.Message}", ex);
            }
        }
    }
}