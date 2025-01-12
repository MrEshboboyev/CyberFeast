using Microsoft.AspNetCore.Authentication;

namespace BuildingBlocks.Security.ApiKey;

/// <summary>
/// Provides extension methods for adding API key support to an <see cref="AuthenticationBuilder"/>.
/// </summary>
public static class AuthenticationBuilderExtensions
{
    /// <summary>
    /// Adds API key support to the <see cref="AuthenticationBuilder"/>.
    /// </summary>
    /// <param name="authenticationBuilder">The <see cref="AuthenticationBuilder"/> to add the API key support to.</param>
    /// <param name="options">The action to configure the <see cref="ApiKeyAuthenticationOptions"/>.</param>
    /// <returns>The configured <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddApiKeySupport(
        this AuthenticationBuilder authenticationBuilder,
        Action<ApiKeyAuthenticationOptions> options
    )
    {
        return authenticationBuilder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
            ApiKeyAuthenticationOptions.DefaultScheme,
            options
        );
    }
}