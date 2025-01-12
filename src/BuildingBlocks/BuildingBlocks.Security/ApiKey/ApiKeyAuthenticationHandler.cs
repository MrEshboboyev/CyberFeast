using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace BuildingBlocks.Security.ApiKey;

/// <summary>
/// Provides logic for handling API key authentication.
/// </summary>
public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IGetApiKeyQuery getApiKeyQuery
) : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder)
{
    private const string ProblemDetailsContentType = "application/problem+json";
    private readonly IGetApiKeyQuery _getApiKeyQuery =
        getApiKeyQuery ?? throw new ArgumentNullException(nameof(getApiKeyQuery));

    /// <summary>
    /// Handles API key authentication by validating the provided API key.
    /// </summary>
    /// <returns>The authentication result.</returns>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        StringValues apiKeyQueryValues = "";

        if (
            Request.Headers.TryGetValue(ApiKeyConstants.HeaderName, out var apiKeyHeaderValues) == false
            && Request.Query.TryGetValue(ApiKeyConstants.HeaderName, out apiKeyQueryValues) == false
        )
            return AuthenticateResult.NoResult();

        var providedApiKey = apiKeyHeaderValues.FirstOrDefault()
                             ?? apiKeyQueryValues.FirstOrDefault();

        if (
            (apiKeyHeaderValues.Count == 0 && apiKeyQueryValues.Count == 0)
            || string.IsNullOrWhiteSpace(providedApiKey)
        )
            return AuthenticateResult.NoResult();

        var existingApiKey = await _getApiKeyQuery.ExecuteAsync(providedApiKey);

        if (existingApiKey == null) return AuthenticateResult.NoResult();

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, existingApiKey.Owner)
        };

        claims.AddRange(existingApiKey.Roles
            .Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
        var identities = new List<ClaimsIdentity> { identity };
        var principal = new ClaimsPrincipal(identities);
        var ticket = new AuthenticationTicket(principal, Options.Scheme);

        return AuthenticateResult.Success(ticket);
    }

    /// <summary>
    /// Handles authentication challenges by providing a 401 Unauthorized response.
    /// </summary>
    /// <param name="properties">The authentication properties.</param>
    /// <returns>A task representing the completion of the challenge handling.</returns>
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = 401;
        Response.ContentType = ProblemDetailsContentType;
        var problemDetails = new UnauthorizedProblemDetails();

        return Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    /// <summary>
    /// Handles forbidden responses by providing a 403 Forbidden response.
    /// </summary>
    /// <param name="properties">The authentication properties.</param>
    /// <returns>A task representing the completion of the forbidden handling.</returns>
    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = 403;
        Response.ContentType = ProblemDetailsContentType;
        var problemDetails = new ForbiddenProblemDetails();

        return Response.WriteAsync(
            JsonSerializer.Serialize(problemDetails, DefaultJsonSerializerOptions.Options));
    }

    /// <summary>
    /// Provides default JSON serializer options.
    /// </summary>
    public static class DefaultJsonSerializerOptions
    {
        /// <summary>
        /// Gets the default JSON serializer options with camel case naming policy.
        /// </summary>
        public static JsonSerializerOptions Options => new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}