using Microsoft.AspNetCore.Authentication;

namespace BuildingBlocks.Security.ApiKey;

/// <summary>
/// Defines the authentication options for the API key scheme.
/// </summary>
public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// The default scheme name for API key authentication.
    /// </summary>
    public const string DefaultScheme = "ApiKey";

    /// <summary>
    /// The authentication type for the API key scheme.
    /// </summary>
    public string AuthenticationType = DefaultScheme;

    /// <summary>
    /// Gets the scheme name for the API key authentication.
    /// </summary>
    public string Scheme => DefaultScheme;
}