using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Web.Problem;

/// <summary>
/// Writes problem details to the HTTP response.
/// </summary>
public class ProblemDetailsWriter : IProblemDetailsWriter
{
    private readonly ProblemDetailsOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProblemDetailsWriter"/> class.
    /// </summary>
    /// <param name="options">The problem details options.</param>
    public ProblemDetailsWriter(IOptions<ProblemDetailsOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Writes the problem details to the HTTP response asynchronously.
    /// </summary>
    /// <param name="context">The problem details context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async ValueTask WriteAsync(ProblemDetailsContext context)
    {
        var httpContext = context.HttpContext;
        TypedResults.Problem(context.ProblemDetails);
        _options.CustomizeProblemDetails?.Invoke(context);

        await httpContext.Response.WriteAsJsonAsync(
            context.ProblemDetails,
            options: null,
            contentType: "application/problem+json"
        );
    }

    /// <summary>
    /// Determines whether the problem details can be written.
    /// </summary>
    /// <param name="context">The problem details context.</param>
    /// <returns><c>true</c> if the problem details can be written; otherwise, <c>false</c>.</returns>
    public bool CanWrite(ProblemDetailsContext context)
    {
        return true;
    }
}