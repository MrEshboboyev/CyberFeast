using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Web.Middlewares.CaptureException;

/// <summary>
/// Middleware implementation for capturing and logging exceptions.
/// </summary>
public class CaptureExceptionMiddlewareImp
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CaptureExceptionMiddlewareImp> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CaptureExceptionMiddlewareImp"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger for logging exceptions.</param>
    public CaptureExceptionMiddlewareImp(
        RequestDelegate next,
        ILogger<CaptureExceptionMiddlewareImp> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Invokes the middleware to capture and log exceptions.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            CaptureException(e, context);
            throw;
        }
    }

    /// <summary>
    /// Captures and logs the exception details.
    /// </summary>
    /// <param name="exception">The exception to capture.</param>
    /// <param name="context">The HTTP context.</param>
    private static void CaptureException(Exception exception, HttpContext context)
    {
        var instance = new ExceptionHandlerFeature
        {
            Path = context.Request.Path,
            Error = exception
        };
        context.Features.Set((IExceptionHandlerPathFeature)instance);
        context.Features.Set((IExceptionHandlerFeature)instance);
    }
}