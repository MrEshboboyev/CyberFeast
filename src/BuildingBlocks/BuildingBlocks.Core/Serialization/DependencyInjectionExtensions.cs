using BuildingBlocks.Abstractions.Serialization;
using BuildingBlocks.Core.Serialization.Converters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BuildingBlocks.Core.Serialization;

/// <summary>
/// Provides methods to configure and register default JSON serializer services with the dependency injection container.
/// </summary>
internal static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds the default JSON serializer services to the service collection.
    /// </summary>
    internal static void AddDefaultSerializer(
        this IServiceCollection services,
        Action<JsonSerializerSettings>? configuration = null
    )
    {
        var defaultSettings = CreateDefaultSerializerSettings();
        configuration?.Invoke(defaultSettings);

        services.AddTransient<ISerializer>(_ => new NewtonsoftObjectSerializer(defaultSettings));
        services.AddTransient<IMessageSerializer>(_ => new NewtonsoftMessageSerializer(defaultSettings));
    }

    /// <summary>
    /// Creates the default JSON serializer settings.
    /// </summary>
    private static JsonSerializerSettings CreateDefaultSerializerSettings(bool camelCase = true, bool indented = false)
    {
        NamingStrategy strategy = camelCase 
            ? new CamelCaseNamingStrategy() 
            : new DefaultNamingStrategy();

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new ContractResolverWithPrivate
            {
                NamingStrategy = strategy
            }
        };

        if (indented)
        {
            settings.Formatting = Formatting.Indented;
        }

        // for handling private constructor
        settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        settings.Converters.Add(new DateOnlyConverter());

        return settings;
    }
}