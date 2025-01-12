using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web.Minimal.Extensions;

/// <summary>
/// Provides extension methods for configuring endpoint conventions.
/// </summary>
public static class EndpointConventionBuilderExtensions
{
    /// <summary>
    /// Adds a response metadata to the endpoint with the specified status code and description.
    /// </summary>
    /// <param name="builder">The endpoint builder.</param>
    /// <param name="description">The description of the response.</param>
    /// <param name="statusCode">The status code of the response.</param>
    /// <param name="responseType">The type of the response.</param>
    /// <param name="contentType">The content type of the response.</param>
    /// <param name="additionalContentTypes">Additional content types accepted by the response.</param>
    /// <returns>The updated endpoint builder.</returns>
    public static RouteHandlerBuilder Produces(
        this RouteHandlerBuilder builder,
        string description,
        int statusCode,
        Type? responseType = null,
        string? contentType = null,
        params string[] additionalContentTypes)
    {
        // Adds OpenAPI metadata and configures response details
        builder.WithOpenApi(operation =>
        {
            operation.Responses[statusCode.ToString(CultureInfo.InvariantCulture)].Description = description;
            return operation;
        });

        builder.Produces(
            statusCode,
            responseType,
            contentType: contentType,
            additionalContentTypes: additionalContentTypes
        );

        return builder;
    }

    /// <summary>
    /// Adds a response metadata with a generic response type to the endpoint with the specified status code and description.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="builder">The endpoint builder.</param>
    /// <param name="description">The description of the response.</param>
    /// <param name="statusCode">The status code of the response.</param>
    /// <param name="contentType">The content type of the response.</param>
    /// <param name="additionalContentTypes">Additional content types accepted by the response.</param>
    /// <returns>The updated endpoint builder.</returns>
    public static RouteHandlerBuilder Produces<TResponse>(
        this RouteHandlerBuilder builder,
        string description,
        int statusCode,
        string? contentType = null,
        params string[] additionalContentTypes)
    {
        // Adds OpenAPI metadata and configures response details
        builder.WithOpenApi(operation =>
        {
            operation.Responses[statusCode.ToString(CultureInfo.InvariantCulture)].Description = description;
            return operation;
        });

        builder.Produces<TResponse>(
            statusCode,
            contentType: contentType,
            additionalContentTypes: additionalContentTypes
        );

        return builder;
    }

    /// <summary>
    /// Adds a problem response metadata to the endpoint with the specified status code and description.
    /// </summary>
    /// <param name="builder">The endpoint builder.</param>
    /// <param name="description">The description of the problem response.</param>
    /// <param name="statusCode">The status code of the problem response.</param>
    /// <param name="contentType">The content type of the problem response.</param>
    /// <returns>The updated endpoint builder.</returns>
    public static RouteHandlerBuilder ProducesProblem(
        this RouteHandlerBuilder builder,
        string description,
        int statusCode,
        string? contentType = null)
    {
        // Adds OpenAPI metadata and configures problem response details
        builder.WithOpenApi(operation =>
        {
            operation.Responses[statusCode.ToString(CultureInfo.InvariantCulture)].Description = description;
            return operation;
        });

        builder.ProducesProblem(statusCode, contentType: contentType);

        return builder;
    }

    /// <summary>
    /// Adds a validation problem response metadata to the endpoint with the specified status code and description.
    /// </summary>
    /// <param name="builder">The endpoint builder.</param>
    /// <param name="description">The description of the validation problem response.</param>
    /// <param name="statusCode">The status code of the validation problem response (default is 400).</param>
    /// <param name="contentType">The content type of the validation problem response.</param>
    /// <returns>The updated endpoint builder.</returns>
    public static RouteHandlerBuilder ProducesValidationProblem(
        this RouteHandlerBuilder builder,
        string description,
        int statusCode = StatusCodes.Status400BadRequest,
        string? contentType = null)
    {
        // Adds OpenAPI metadata and configures validation problem response details
        builder.WithOpenApi(operation =>
        {
            operation.Responses[statusCode.ToString(CultureInfo.InvariantCulture)].Description = description;
            return operation;
        });

        builder.ProducesValidationProblem(statusCode, contentType: contentType);

        return builder;
    }

    /// <summary>
    /// Adds a summary and description to the endpoint.
    /// </summary>
    /// <param name="builder">The endpoint builder.</param>
    /// <param name="summary">The summary of the endpoint.</param>
    /// <param name="description">The description of the endpoint.</param>
    /// <returns>The updated endpoint builder.</returns>
    public static RouteHandlerBuilder WithSummaryAndDescription(
        this RouteHandlerBuilder builder,
        string summary,
        string description)
    {
        // Adds OpenAPI metadata for summary and description
        builder.WithOpenApi(operation =>
        {
            operation.Summary = summary;
            operation.Description = description;
            return operation;
        });

        return builder;
    }
}