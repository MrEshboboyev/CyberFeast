using System.Reflection;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Extensions.ServiceCollection;
using BuildingBlocks.Core.Messaging.BackgroundServices;
using BuildingBlocks.Core.Messaging.MessagePersistence;
using BuildingBlocks.Core.Messaging.MessagePersistence.InMemory;
using BuildingBlocks.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scrutor;

namespace BuildingBlocks.Core.Messaging.Extensions;

/// <summary>
/// Provides extension methods for registering messaging-related services with the dependency injection container.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds core messaging services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="scanAssemblies">Optional assemblies to scan for messaging services. Defaults to referenced assemblies of the calling assembly.</param>
    internal static void AddMessagingCore(
        this IServiceCollection services, 
        params Assembly[] scanAssemblies)
    {
        var assemblies = scanAssemblies.Length != 0
            ? scanAssemblies
            : ReflectionUtilities.GetReferencedAssemblies(Assembly.GetCallingAssembly()).Distinct().ToArray();

        services.AddScoped<IMessageMetadataAccessor, MessageMetadataAccessor>();
        AddMessagingMediator(services, assemblies);
        AddPersistenceMessage(services);
    }

    /// <summary>
    /// Adds message persistence services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    private static void AddPersistenceMessage(IServiceCollection services)
    {
        services.TryAddScoped<IMessagePersistenceService, MessagePersistenceService>();
        services.AddHostedService<MessagePersistenceBackgroundService>();
        services.AddValidatedOptions<MessagePersistenceOptions>();
        services.AddInMemoryMessagePersistence();
    }

    /// <summary>
    /// Adds in-memory message persistence services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddInMemoryMessagePersistence(
        this IServiceCollection services)
    {
        services.Replace(
            ServiceDescriptor.Scoped<IMessagePersistenceRepository, InMemoryMessagePersistenceRepository>()
        );

        return services;
    }

    /// <summary>
    /// Adds messaging MediatR services to the service collection and scans specified assemblies for message handlers.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="scanAssemblies">The assemblies to scan for message handlers.</param>
    private static void AddMessagingMediator(
        IServiceCollection services, 
        Assembly[] scanAssemblies)
    {
        services.Scan(scan =>
            scan.FromAssemblies(scanAssemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IMessageHandler<>)))
                .UsingRegistrationStrategy(RegistrationStrategy.Append)
                .AsClosedTypeOf(typeof(IMessageHandler<>))
                .AsSelf()
                .WithLifetime(ServiceLifetime.Transient)
        );
    }
}