using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Resiliency.Extensions;

/// <summary>
/// Provides extension methods for registering services with custom policies.
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers an HTTP API client with custom policy handlers.
    /// </summary>
    /// <typeparam name="TInterface">The interface type of the API client.</typeparam>
    /// <typeparam name="TClient">The implementation type of the API client.</typeparam>
    /// <param name="services">The service collection to add the client to.</param>
    /// <param name="configureClient">An action to configure the client.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddHttpApiClient<TInterface, TClient>(
        this IServiceCollection services,
        Action<IServiceProvider, HttpClient> configureClient
    )
        where TInterface : class
        where TClient : class, TInterface
    {
        services.AddHttpClient<TInterface, TClient>(configureClient).AddCustomPolicyHandlers();

        return services;
    }
}