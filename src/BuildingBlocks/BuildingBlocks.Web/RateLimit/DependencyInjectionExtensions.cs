using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web.RateLimit;

/// <summary>
/// Provides methods to add and use custom rate limiting in an ASP.NET Core application.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds custom rate limiting services to the <see cref="WebApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <returns>The updated web application builder.</returns>
    public static WebApplicationBuilder AddCustomRateLimit(this WebApplicationBuilder builder)
    {
        // Configure rate limiting options
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Apply a global rate limiter with a fixed window of 10 requests per minute per user or host
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name 
                                  ?? httpContext.Request.Headers.Host.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }
                )
            );
        });

        return builder;
    }

    /// <summary>
    /// Uses custom rate limiting in the web application.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The updated web application.</returns>
    public static WebApplication UseCustomRateLimit(this WebApplication app)
    {
        app.UseRateLimiter();
        return app;
    }
}