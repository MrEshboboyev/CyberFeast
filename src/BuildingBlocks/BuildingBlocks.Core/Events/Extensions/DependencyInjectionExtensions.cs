using System.Reflection;
using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Events.Internal;
using BuildingBlocks.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Events.Extensions;

/// <summary>
/// Provides extension methods for setting up dependency injection for event handling.
/// </summary>
internal static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds event bus services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="scanAssemblies">The assemblies to scan for event handlers and mappers.</param>
    /// <returns>The updated service collection.</returns>
    internal static IServiceCollection AddEventBus(
        this IServiceCollection services,
        params Assembly[] scanAssemblies)
    {
        var assemblies =
            scanAssemblies.Length != 0
                ? scanAssemblies
                : ReflectionUtilities
                    .GetReferencedAssemblies(Assembly.GetCallingAssembly()).Distinct().ToArray();

        services
            .AddTransient<IDomainEventPublisher, DomainEventPublisher>()
            .AddTransient<IDomainNotificationEventPublisher, DomainNotificationEventPublisher>()
            .AddTransient<IInternalEventBus, InternalEventBus>();

        services.AddTransient<IAggregatesDomainEventsRequestStorage, AggregatesDomainEventsStorage>();
        services.AddScoped<IDomainEventsAccessor, DomainEventAccessor>();

        RegisterEventMappers(services, assemblies);

        return services;
    }

    /// <summary>
    /// Registers event mappers in the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="scanAssemblies">The assemblies to scan for event mappers.</param>
    private static void RegisterEventMappers(IServiceCollection services, Assembly[] scanAssemblies)
    {
        services.Scan(scan =>
            scan.FromAssemblies(scanAssemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IEventMapper)), false)
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IIntegrationEventMapper)), false)
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IIDomainNotificationEventMapper)), false)
                .AsImplementedInterfaces()
                .WithSingletonLifetime());
    }
}