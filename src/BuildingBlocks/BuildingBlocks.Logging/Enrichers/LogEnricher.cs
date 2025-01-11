using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;

namespace BuildingBlocks.Logging.Enrichers;

/// <summary>
/// Provides methods to enrich logs with information from <see cref="HttpContext"/>.
/// </summary>
public static class LogEnricher
{
    /// <summary>
    /// Enriches the diagnostic context with information from the request and response.
    /// </summary>
    /// <param name="diagnosticContext">The diagnostic context.</param>
    /// <param name="httpContext">The HTTP context.</param>
    public static void EnrichFromRequest(
        IDiagnosticContext diagnosticContext,
        HttpContext httpContext)
    {
        var request = httpContext.Request;

        diagnosticContext.Set("Host", request.Host);
        diagnosticContext.Set("Protocol", request.Protocol);
        diagnosticContext.Set("Scheme", request.Scheme);

        if (request.QueryString.HasValue)
        {
            diagnosticContext.Set("QueryString", request.QueryString.Value);
        }

        diagnosticContext.Set("ContentType", httpContext.Response.ContentType);
        diagnosticContext.Set("UserId", httpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value);

        var endpoint = httpContext.GetEndpoint();
        if (endpoint != null)
        {
            diagnosticContext.Set("EndpointName", endpoint.DisplayName);
        }
    }

    /// <summary>
    /// Determines the log level based on the <see cref="HttpContext"/> and any exceptions.
    /// </summary>
    /// <param name="ctx">The HTTP context.</param>
    /// <param name="_">The elapsed request time.</param>
    /// <param name="ex">The exception, if any.</param>
    /// <returns>The log level.</returns>
    public static LogEventLevel GetLogLevel(HttpContext ctx, double _, Exception? ex) =>
        ex != null
            ? LogEventLevel.Error
            : ctx.Response.StatusCode > 499
                ? LogEventLevel.Error
                : IsHealthCheckEndpoint(ctx) || IsSwagger(ctx)
                    ? LogEventLevel.Debug
                    : LogEventLevel.Information;

    private static bool IsSwagger(HttpContext ctx)
    {
        var isSwagger = ctx.Request.Path.Value?
            .Contains("swagger", StringComparison.Ordinal) ?? false;
        return isSwagger;
    }

    private static bool IsHealthCheckEndpoint(HttpContext ctx)
    {
        var isHealth = ctx.Request.Path.Value?
            .Contains("health", StringComparison.Ordinal) ?? false;
        return isHealth;
    }
}