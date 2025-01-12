using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Web.Problem.HttpResults;

/// <summary>
/// Represents an HTTP result for a 403 Forbidden response with a problem details object.
/// </summary>
public class ForbiddenHttpProblemResult : IResult, IStatusCodeHttpResult, IContentTypeHttpResult, IValueHttpResult,
    IEndpointMetadataProvider
{
    private readonly ProblemHttpResult _internalResult;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenHttpProblemResult"/> class.
    /// </summary>
    /// <param name="problemDetails">The problem details object.</param>
    /// <exception cref="ArgumentNullException">Thrown if the problem details object is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the status code is not 403 Forbidden.</exception>
    internal ForbiddenHttpProblemResult(ProblemDetails problemDetails)
    {
        ArgumentNullException.ThrowIfNull(problemDetails);
        if (problemDetails is { Status: not null and not StatusCodes.Status403Forbidden })
        {
            throw new ArgumentException(
                $"{nameof(ForbiddenHttpProblemResult)} only supports a 403 Forbidden response status code.",
                nameof(problemDetails)
            );
        }

        problemDetails.Status ??= StatusCodes.Status403Forbidden;

        _internalResult = TypedResults.Problem(problemDetails);
    }

    /// <summary>
    /// Gets the problem details object.
    /// </summary>
    public ProblemDetails ProblemDetails => _internalResult.ProblemDetails;

    /// <summary>
    /// Executes the result asynchronously.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ExecuteAsync(HttpContext context)
    {
        return _internalResult.ExecuteAsync(context);
    }

    /// <summary>
    /// Gets the status code for the response.
    /// </summary>
    public int? StatusCode => _internalResult.StatusCode;

    /// <summary>
    /// Gets the content type for the response.
    /// </summary>
    public string? ContentType => _internalResult.ContentType;

    object? IValueHttpResult.Value => _internalResult.ProblemDetails;

    /// <summary>
    /// Populates endpoint metadata for the result.
    /// </summary>
    /// <param name="method">The method information.</param>
    /// <param name="builder">The endpoint builder.</param>
    /// <exception cref="ArgumentNullException">Thrown if the method or builder is null.</exception>
    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(builder);
        builder.Metadata.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status403Forbidden));
    }
}