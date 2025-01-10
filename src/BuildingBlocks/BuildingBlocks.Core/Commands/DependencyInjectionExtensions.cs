using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Scheduler;
using BuildingBlocks.Core.Scheduler;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Commands;

/// <summary>
/// Provides extension methods for registering command-related services with the dependency injection container.
/// </summary>
internal static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds command bus services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCommandBus(this IServiceCollection services)
    {
        services.AddTransient<ICommandBus, CommandBus>();
        services.AddTransient<IAsyncCommandBus, AsyncCommandBus>();
        services.AddTransient<ICommandScheduler, NullCommandScheduler>();

        return services;
    }
}