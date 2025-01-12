using BuildingBlocks.Core.Web.HeaderPropagation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace BuildingBlocks.Web.Middlewares.HeaderPropagation;

/// <summary>
/// Middleware to propagate headers from incoming requests to the HeaderPropagationStore.
/// </summary>
public class HeaderPropagationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly CustomHeaderPropagationOptions _options;
    private readonly HeaderPropagationStore _headerPropagationStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeaderPropagationMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="options">The custom header propagation options.</param>
    /// <param name="headerPropagationStore">The header propagation store.</param>
    /// <exception cref="ArgumentNullException">Thrown if any of the parameters are null.</exception>
    public HeaderPropagationMiddleware(
        RequestDelegate next,
        IOptions<CustomHeaderPropagationOptions> options,
        HeaderPropagationStore headerPropagationStore)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(headerPropagationStore);

        _next = next;
        _options = options.Value;
        _headerPropagationStore = headerPropagationStore;
    }

    /// <summary>
    /// Invokes the middleware to propagate headers from the incoming request to the HeaderPropagationStore.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Invoke(HttpContext context)
    {
        foreach (var headerName in _options.HeaderNames)
        {
            if (_headerPropagationStore.Headers.ContainsKey(headerName)) continue;

            context.Request.Headers.TryGetValue(headerName, out var value);
            if (!StringValues.IsNullOrEmpty(value))
            {
                _headerPropagationStore.Headers.Add(headerName, value);
            }
        }

        await _next.Invoke(context);
    }
}