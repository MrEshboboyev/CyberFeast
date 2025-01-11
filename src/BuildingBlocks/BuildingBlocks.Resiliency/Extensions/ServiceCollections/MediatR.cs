using System.Reflection;
using BuildingBlocks.Resiliency.Fallback;
using BuildingBlocks.Resiliency.Retry;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scrutor;

namespace BuildingBlocks.Resiliency.Extensions;

/// <summary>
/// Provides extension methods for registering MediatR behaviors with custom policies.
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a retry policy for MediatR requests, scanning assemblies for retryable request implementations.
    /// </summary>
    /// <param name="services">The service collection to add the policies to.</param>
    /// <param name="assemblies">The list of assemblies to scan for retryable requests.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMediaterRetryPolicy(
        IServiceCollection services,
        IReadOnlyList<Assembly> assemblies
    )
    {
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));

        services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IRetryableRequest<,>)))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithTransientLifetime()
        );

        return services;
    }

    /// <summary>
    /// Adds a fallback policy for MediatR requests, scanning assemblies for fallback handler implementations.
    /// </summary>
    /// <param name="services">The service collection to add the policies to.</param>
    /// <param name="assemblies">The list of assemblies to scan for fallback handlers.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMediaterFallbackPolicy(
        IServiceCollection services,
        IReadOnlyList<Assembly> assemblies
    )
    {
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(FallbackBehavior<,>));

        services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IFallbackHandler<,>)))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithTransientLifetime()
        );

        return services;
    }
}