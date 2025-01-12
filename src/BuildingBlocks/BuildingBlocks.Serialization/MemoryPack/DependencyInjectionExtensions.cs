using BuildingBlocks.Abstractions.Serialization;
using MemoryPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Serialization.MemoryPack;

/// <summary>
/// Provides extension methods for registering MemoryPack serialization services.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds MemoryPack serialization services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configurator">Optional action to configure MemoryPack serializer options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMemoryPackSerialization(
        this IServiceCollection services,
        Action<MemoryPackSerializerOptions>? configurator = null
    )
    {
        var serializerOptions = MemoryPackSerializerOptions.Default;
        configurator?.Invoke(serializerOptions);

        // Register MemoryPackObjectSerializer for object serialization.
        services.Replace(
            ServiceDescriptor.Transient<ISerializer>(_ => new MemoryPackObjectSerializer(serializerOptions))
        );

        // Register MemoryPackMessageSerializer for message serialization.
        services.Replace(
            ServiceDescriptor.Transient<IMessageSerializer>(_ => new MemoryPackMessageSerializer(serializerOptions))
        );

        return services;
    }
}