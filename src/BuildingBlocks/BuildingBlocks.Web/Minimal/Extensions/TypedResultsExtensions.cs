using BuildingBlocks.Web.Problem.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Web.Minimal.Extensions;

/// <summary>
/// Provides extension methods for creating custom problem results.
/// </summary>
public static class TypedResultsExtensions
{
    /// <summary>
    /// Creates a custom problem result for a 500 Internal Server Error response.
    /// </summary>
    /// <param name="title">The title of the problem.</param>
    /// <param name="detail">The detail of the problem.</param>
    /// <param name="instance">The instance of the problem.</param>
    /// <param name="type">The type of the problem.</param>
    /// <param name="extensions">Additional extensions for the problem details.</param>
    /// <returns>An instance of <see cref="InternalHttpProblemResult"/>.</returns>
    public static InternalHttpProblemResult InternalProblem(
        string? title = null,
        string? detail = null,
        string? instance = null,
        string? type = null,
        IDictionary<string, object?>? extensions = null)
    {
        var problemDetails = CreateProblem(title, detail, instance, type, extensions);

        return new InternalHttpProblemResult(problemDetails);
    }

    /// <summary>
    /// Creates a custom problem result for a 401 Unauthorized response.
    /// </summary>
    /// <param name="title">The title of the problem.</param>
    /// <param name="detail">The detail of the problem.</param>
    /// <param name="instance">The instance of the problem.</param>
    ///         /// <param name="type">The type of the problem.</param>
    /// <param name="extensions">Additional extensions for the problem details.</param>
    /// <returns>An instance of <see cref="UnAuthorizedHttpProblemResult"/>.</returns>
    public static UnAuthorizedHttpProblemResult UnAuthorizedProblem(
        string? title = null,
        string? detail = null,
        string? instance = null,
        string? type = null,
        IDictionary<string, object?>? extensions = null)
    {
        var problemDetails = CreateProblem(title, detail, instance, type, extensions);

        return new UnAuthorizedHttpProblemResult(problemDetails);
    }

    /// <summary>
    /// Creates a ProblemDetails object with the provided details and extensions.
    /// </summary>
    /// <param name="title">The title of the problem.</param>
    /// <param name="detail">The detail of the problem.</param>
    /// <param name="instance">The instance of the problem.</param>
    /// <param name="type">The type of the problem.</param>
    /// <param name="extensions">Additional extensions for the problem details.</param>
    /// <returns>An instance of <see cref="ProblemDetails"/>.</returns>
    private static ProblemDetails CreateProblem(
        string? title,
        string? detail,
        string? instance,
        string? type,
        IDictionary<string, object?>? extensions)
    {
        var problemDetails = new ProblemDetails
        {
            Detail = detail,
            Instance = instance,
            Type = type
        };

        problemDetails.Title = title ?? problemDetails.Title;

        if (extensions is null) return problemDetails;

        foreach (var extension in extensions)
        {
            problemDetails.Extensions.Add(extension);
        }

        return problemDetails;
    }
}