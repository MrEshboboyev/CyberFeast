using BuildingBlocks.Core.Web.HeaderPropagation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web.Middlewares.HeaderPropagation;

/// <summary>
/// Provides extension methods for adding the header propagation middleware to the request pipeline.
/// </summary>
public static class HeaderPropagationApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the header propagation middleware to the request pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The updated application builder.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the application builder is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the HeaderPropagationStore is not registered.</exception>
    public static IApplicationBuilder UseHeaderPropagation(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (app.ApplicationServices.GetService<HeaderPropagationStore>() == null)
        {
            throw new InvalidOperationException(
                "CustomHeaderPropagationStore not registered. Please add it with AddHeaderPropagation");
        }

        return app.UseMiddleware<HeaderPropagationMiddleware>();
    }
}