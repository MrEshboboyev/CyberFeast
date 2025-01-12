using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Web.Middlewares.CaptureException;

/// <summary>
/// Provides extension methods for adding the capture exception middleware to the request pipeline.
/// </summary>
public static class CaptureExceptionMiddlewareExtensions
{
    /// <summary>
    /// Adds the capture exception middleware to the request pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The updated application builder.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the application builder is null.</exception>
    public static IApplicationBuilder UseCaptureException(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.Properties["analysis.NextMiddlewareName"] = "Shared.Web.Middlewares.CaptureExceptionMiddleware";
        return app.UseMiddleware<CaptureExceptionMiddlewareImp>();
    }
}