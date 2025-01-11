using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Logging.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="HttpContext"/> to retrieve metrics-related information.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Gets the current resource name for metrics from the HttpContext.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The current resource name.</returns>
        public static string? GetMetricsCurrentResourceName(this HttpContext httpContext)
        {
            ArgumentNullException.ThrowIfNull(httpContext);

            var endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;
            return endpoint?.Metadata.GetMetadata<EndpointNameMetadata>()?.EndpointName;
        }
    }
}