using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Core.Extensions.ServiceCollection;

/// <summary>
/// Provides extension methods for the IServiceCollection interface to add and validate configuration options.
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds configuration options of the specified type <typeparamref name="T"/> to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="key">The configuration key to bind. Defaults to the name of the type <typeparamref name="T"/>.</param>
    /// <param name="configurator">An optional action to configure the options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddConfigurationOptions<T>(
        this IServiceCollection services,
        string? key = null,
        Action<T>? configurator = null
    )
        where T : class
    {
        var optionBuilder = services
            .AddOptions<T>()
            .BindConfiguration(key ?? typeof(T).Name);

        if (configurator is not null)
        {
            optionBuilder = optionBuilder.Configure(configurator);
        }

        services.TryAddSingleton(
            x => x.GetRequiredService<IOptions<T>>().Value);

        return services;
    }

    /// <summary>
    /// Adds and validates configuration options of the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configurator">An optional action to configure the options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddValidationOptions<T>(
        this IServiceCollection services,
        Action<T>? configurator = null
    )
        where T : class
    {
        var key = typeof(T).Name;

        return AddValidatedOptions(
            services,
            key,
            RequiredConfigurationValidator.Validate,
            configurator);
    }

    /// <summary>
    /// Adds and validates configuration options of the specified type <typeparamref name="T"/> with an optional configuration key and configurator.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="key">The configuration key to bind. Defaults to the name of the type <typeparamref name="T"/>.</param>
    /// <param name="configurator">An optional action to configure the options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddValidationOptions<T>(
        this IServiceCollection services,
        string? key = null,
        Action<T>? configurator = null
    )
        where T : class
    {
        return AddValidatedOptions(
            services,
            key ?? typeof(T).Name,
            RequiredConfigurationValidator.Validate,
            configurator
        );
    }

    /// <summary>
    /// Adds and validates configuration options of the specified type <typeparamref name="T"/> with the provided validator, key, and configurator.
    /// </summary>
    /// <typeparam name="T">The type of the options to add and validate.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="key">The configuration key to bind. Defaults to the name of the type <typeparamref name="T"/>.</param>
    /// <param name="validator">A function to validate the options.</param>
    /// <param name="configurator">An optional action to configure the options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddValidatedOptions<T>(
        this IServiceCollection services,
        string? key = null,
        Func<T, bool>? validator = null,
        Action<T>? configurator = null
    )
        where T : class
    {
        validator ??= RequiredConfigurationValidator.Validate;

        var optionBuilder = services.AddOptions<T>().BindConfiguration(key ?? typeof(T).Name);

        if (configurator is not null)
        {
            optionBuilder = optionBuilder.Configure(configurator);
        }

        optionBuilder.Validate(validator);

        // IOptions<T> itself registered as singleton
        services.TryAddSingleton(x => x.GetRequiredService<IOptions<T>>().Value);

        return services;
    }
}

/// <summary>
/// Provides methods to validate required configuration properties.
/// </summary>
public static class RequiredConfigurationValidator
{
    /// <summary>
    /// Validates that all properties marked with the RequiredMemberAttribute are not null.
    /// </summary>
    /// <typeparam name="T">The type of the options to validate.</typeparam>
    /// <param name="arg">The options instance to validate.</param>
    /// <returns>True if all required properties are not null, otherwise throws an exception.</returns>
    public static bool Validate<T>(T arg)
        where T : class
    {
        var requiredProperties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => Attribute.IsDefined(x, typeof(RequiredMemberAttribute)));

        foreach (var requiredProperty in requiredProperties)
        {
            var propertyValue = requiredProperty.GetValue(arg);
            if (propertyValue is null)
            {
                throw new System.Exception($"Required property '{requiredProperty.Name}' was null");
            }
        }

        return true;
    }
}