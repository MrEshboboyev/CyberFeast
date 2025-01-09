using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Core.Web.HeaderPropagation.Extensions;

/// <summary>
/// Provides methods to register header propagation services with the dependency injection container.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Applies header propagation to all HTTP clients.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configureOptions">A delegate to configure the header propagation options.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddHeaderPropagation(this IServiceCollection services,
        Action<CustomHeaderPropagationOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.AddHeaderPropagationCore(configureOptions);

        // Apply the message handler to all HTTP clients
        services.TryAddEnumerable(ServiceDescriptor
            .Singleton<IHttpMessageHandlerBuilderFilter, HeaderPropagationMessageHandlerBuilderFilter>());

        return services;
    }

    /// <summary>
    /// Applies header propagation to a specific HTTP client.
    /// </summary>
    /// <param name="builder">The HTTP client builder.</param>
    /// <param name="configure">A delegate to configure the header propagation options.</param>
    /// <returns>The HTTP client builder.</returns>
    public static IHttpClientBuilder AddHeaderPropagation(this IHttpClientBuilder builder,
        Action<CustomHeaderPropagationOptions> configure)
    {
        builder.Services.AddHeaderPropagationCore(configure);
        builder.AddHttpMessageHandler(sp =>
        {
            var options = sp.GetRequiredService<IOptions<CustomHeaderPropagationOptions>>();
            var headers = sp.GetRequiredService<HeaderPropagationStore>();

            return new HeaderPropagationMessageHandler(options.Value, headers);
        });

        return builder;
    }

    /// <summary>
    /// Core method to add header propagation services and configuration to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">A delegate to configure the header propagation options.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddHeaderPropagationCore(this IServiceCollection services,
        Action<CustomHeaderPropagationOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.TryAddSingleton<HeaderPropagationStore>();

        services.Configure(configureOptions);

        return services;
    }
}