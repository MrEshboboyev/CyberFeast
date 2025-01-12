using BuildingBlocks.Security.ApiKey.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Security.ApiKey;

/// <summary>
/// Provides extension methods for registering custom API key authentication and authorization services.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds custom API key authentication and authorization to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCustomApiKeyAuthentication(this IServiceCollection services)
    {
        // Configure authentication using API key scheme.
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
            })
            .AddApiKeySupport(_ => { });

        // Configure authorization policies for customers, admins, and third parties.
        services.AddAuthorizationBuilder()
            .AddPolicy(
                Policies.OnlyCustomers,
                policy => policy.Requirements.Add(new OnlyCustomersRequirement())
            )
            .AddPolicy(
                Policies.OnlyAdmins,
                policy => policy.Requirements.Add(new OnlyAdminsRequirement())
            )
            .AddPolicy(
                Policies.OnlyThirdParties,
                policy => policy.Requirements.Add(new OnlyThirdPartiesRequirement())
            );

        // Register authorization handlers.
        services.TryAddSingleton<IAuthorizationHandler, OnlyCustomersAuthorizationHandler>();
        services.TryAddSingleton<IAuthorizationHandler, OnlyAdminsAuthorizationHandler>();
        services.TryAddSingleton<IAuthorizationHandler, OnlyThirdPartiesAuthorizationHandler>();

        // Register in-memory query for retrieving API keys.
        services.TryAddSingleton<IGetApiKeyQuery, InMemoryGetApiKeyQuery>();

        return services;
    }
}