using BuildingBlocks.Abstractions.Caching;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Extensions.ServiceCollection;
using EasyCaching.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace BuildingBlocks.Caching.Extensions;

/// <summary>
/// Provides extension methods for configuring caching and Redis services.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Configures EasyCaching with options for Redis and in-memory caching.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configurator">An optional action to configure the cache options.</param>
    /// <returns>The updated web application builder.</returns>
    public static WebApplicationBuilder AddCustomEasyCaching(
        this WebApplicationBuilder builder,
        Action<CacheOptions>? configurator = null
    )
    {
        var cacheOptions = builder.Configuration.BindOptions<CacheOptions>();
        configurator?.Invoke(cacheOptions);

        // Add option to the dependency injection
        builder.Services.AddValidationOptions<CacheOptions>(opt => configurator?.Invoke(opt));

        builder.Services.AddEasyCaching(option =>
        {
            if (cacheOptions.RedisCacheOptions is not null)
            {
                option.UseRedis(
                    config =>
                    {
                        config.DBConfig = new RedisDBOptions
                        {
                            Configuration = cacheOptions.RedisCacheOptions.ConnectionString
                        };
                        config.SerializerName = cacheOptions.SerializationType;
                    },
                    nameof(CacheProviderType.Redis)
                );
            }

            option.UseInMemory(
                config => { config.SerializerName = cacheOptions.SerializationType; },
                nameof(CacheProviderType.InMemory)
            );

            switch (cacheOptions.SerializationType)
            {
                case nameof(CacheSerializationType.Json):
                    option.WithJson(
                        jsonSerializerSettingsConfigure: x =>
                        {
                            x.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None;
                        },
                        nameof(CacheSerializationType.Json)
                    );
                    break;
                case nameof(CacheSerializationType.MessagePack):
                    option.WithMessagePack(nameof(CacheSerializationType.MessagePack));
                    break;
            }
        });

        return builder;
    }

    /// <summary>
    /// Configures Redis with connection settings.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configurator">An optional action to configure the Redis options.</param>
    /// <returns>The updated web application builder.</returns>
    public static WebApplicationBuilder AddCustomRedis(
        this WebApplicationBuilder builder,
        Action<RedisOptions>? configurator = null
    )
    {
        var redisOptions = builder.Configuration.BindOptions<RedisOptions>();
        configurator?.Invoke(redisOptions);

        builder.Services.AddValidationOptions<RedisOptions>(
            opt => configurator?.Invoke(opt));

        builder.Services.TryAddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    EndPoints =
                    {
                        $"{redisOptions.Host}:{redisOptions.Port}"
                    },
                    AllowAdmin = true
                }
            )
        );

        return builder;
    }
}