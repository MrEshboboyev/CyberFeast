using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Abstractions.Web.MinimalApi;

/// <summary>
/// Defines the structure for a minimal API endpoint.
/// </summary>
public interface IMinimalEndpoint
{
    /// <summary>
    /// Gets the name of the endpoint group.
    /// </summary>
    string GroupName { get; }

    /// <summary>
    /// Gets the prefix route for the endpoint.
    /// </summary>
    string PrefixRoute { get; }

    /// <summary>
    /// Gets the version of the endpoint.
    /// </summary>
    double Version { get; }

    /// <summary>
    /// Maps the endpoint to an endpoint route builder.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <returns>A route handler builder.</returns>
    RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder builder);
}

/// <summary>
/// Defines the structure for a minimal API endpoint that returns a result.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IMinimalEndpoint<TResult> : IMinimalEndpoint
{
    /// <summary>
    /// Asynchronously handles an HTTP request and returns a result.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task that represents the asynchronous operation and returns a result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> HandleAsync(HttpContext context);
}

/// <summary>
/// Defines the structure for a minimal API endpoint that handles requests with a specified request type.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IMinimalEndpoint<in TRequest, TResult> : IMinimalEndpoint
{
    /// <summary>
    /// Asynchronously handles an HTTP request with a specified request type and returns a result.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and returns a result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> HandleAsync(
        HttpContext context,
        TRequest request,
        CancellationToken cancellationToken);
}

/// <summary>
/// Defines the structure for a minimal API endpoint that handles requests with a specified request type and dependency.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TDependency">The type of the dependency.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IMinimalEndpoint<in TRequest, in TDependency, TResult> : IMinimalEndpoint
{
    /// <summary>
    /// Asynchronously handles an HTTP request with a specified request type and dependency, and returns a result.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="request">The request.</param>
    /// <param name="dependency">The dependency.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and returns a result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> HandleAsync(
        HttpContext context,
        TRequest request,
        TDependency dependency,
        CancellationToken cancellationToken);
}

/// <summary>
/// Defines the structure for a minimal API endpoint that handles requests with a specified request type and multiple dependencies.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TDependency1">The type of the first dependency.</typeparam>
/// <typeparam name="TDependency2">The type of the second dependency.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IMinimalEndpoint<in TRequest, in TDependency1, in TDependency2, TResult> : IMinimalEndpoint
{
    /// <summary>
    /// Asynchronously handles an HTTP request with a specified request type and multiple dependencies, and returns a result.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="request">The request.</param>
    /// <param name="dependency1">The first dependency.</param>
    /// <param name="dependency2">The second dependency.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and returns a result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> HandleAsync(
        HttpContext context,
        TRequest request,
        TDependency1 dependency1,
        TDependency2 dependency2,
        CancellationToken cancellationToken);
}

/// <summary>
/// Defines the structure for a minimal API endpoint that handles requests with a specified request type and multiple dependencies.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TDependency1">The type of the first dependency.</typeparam>
/// <typeparam name="TDependency2">The type of the second dependency.</typeparam>
/// <typeparam name="TDependency3">The type of the third dependency.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface
    IMinimalEndpoint<in TRequest, in TDependency1, in TDependency2, in TDependency3, TResult> : IMinimalEndpoint
{
    /// <summary>
    /// Asynchronously handles an HTTP request with a specified request type and multiple dependencies, and returns a result.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="request">The request.</param>
    /// <param name="dependency1">The first dependency.</param>
    /// <param name="dependency2">The second dependency.</param>
    /// <param name="dependency3">The third dependency.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and returns a result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> HandleAsync(
        HttpContext context,
        TRequest request,
        TDependency1 dependency1,
        TDependency2 dependency2,
        TDependency3 dependency3,
        CancellationToken cancellationToken);
}

/// <summary>
/// Defines the structure for a minimal API endpoint that handles command requests with specified request parameters.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TRequestParameters">The type of the request parameters.</typeparam>
public interface ICommandMinimalEndpoint<in TRequest, in TRequestParameters> : IMinimalEndpoint
    where TRequestParameters : IHttpCommand<TRequest>
{
    /// <summary>
    /// Asynchronously handles a command request with specified request parameters.
    /// </summary>
    /// <param name="requestParameters">The request parameters.</param>
    /// <returns>A task that represents the asynchronous operation and returns an <see cref="IResult"/>.</returns>
    Task<IResult> HandleAsync([AsParameters] TRequestParameters requestParameters);
}

/// <summary>
/// Defines the structure for a minimal API endpoint that handles command requests with specified request parameters and returns a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TRequestParameters">The type of the request parameters.</typeparam>
/// <typeparam name="TResult1">The type of the first result.</typeparam>
public interface ICommandMinimalEndpoint<in TRequest, in TRequestParameters, TResult1> : IMinimalEndpoint
    where TRequestParameters : IHttpCommand<TRequest>
    where TResult1 : IResult
{
    /// <summary>
    /// Asynchronously handles a command request with specified request parameters and returns a result.
    /// </summary>
    /// <param name="requestParameters">The request parameters.</param>
    /// <returns>A task that represents the asynchronous operation and returns a result of type <typeparamref name="TResult1"/>.</returns>
    Task<TResult1> HandleAsync([AsParameters] TRequestParameters requestParameters);
}

/// <summary>
/// Defines the structure for a minimal API endpoint that handles command requests with specified request parameters and returns multiple results.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TRequestParameters">The type of the request parameters.</typeparam>
/// <typeparam name="TResult1">The type of the first result.</typeparam>
/// <typeparam name="TResult2">The type of the second result.</typeparam>
public interface ICommandMinimalEndpoint<in TRequest, in TRequestParameters, TResult1, TResult2> : IMinimalEndpoint
    where TRequestParameters : IHttpCommand<TRequest>
    where TResult1 : IResult
    where TResult2 : IResult
{
    /// <summary>
    /// Asynchronously handles a command request with specified request parameters and returns multiple results.
    /// </summary>
    /// <param name="requestParameters">The request parameters.</param>
    /// <returns>A task that represents the asynchronous operation and returns multiple results.</returns>
    Task<Results<TResult1, TResult2>> HandleAsync([AsParameters] TRequestParameters requestParameters);
}

/// <summary>
/// Defines the structure for a minimal API endpoint that handles command requests with specified request parameters and returns multiple results.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TRequestParameters">The type of the request parameters.</typeparam>
/// <typeparam name="TResult1">The type of the first result.</typeparam>
/// <typeparam name="TResult2">The type of the second result.</typeparam>
/// <typeparam name="TResult3">The type of the third result.</typeparam>
public interface
    ICommandMinimalEndpoint<in TRequest, in TRequestParameters, TResult1, TResult2, TResult3> : IMinimalEndpoint
    where TRequestParameters : IHttpCommand<TRequest>
    where TResult1 : IResult
    where TResult2 : IResult
    where TResult3 : IResult
{
    /// <summary>
    /// Asynchronously handles a command request with specified request parameters and returns multiple results.
    /// </summary>
    /// <param name="requestParameters">The request parameters.</param>
    /// <returns>A task that represents the asynchronous operation and returns multiple results.</returns>
    Task<Results<TResult1, TResult2, TResult3>> HandleAsync([AsParameters] TRequestParameters requestParameters);
}

/// <summary>
/// Defines the structure for a minimal API endpoint that handles command requests with specified request parameters and returns multiple results.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TRequestParameters">The type of the request parameters.</typeparam>
/// <typeparam name="TResult1">The type of the first result.</typeparam>
/// <typeparam name="TResult2">The type of the second result.</typeparam>
/// <typeparam name="TResult3">The type of the third result.</typeparam>
/// <typeparam name="TResult4">The type of the fourth result.</typeparam>
public interface
    ICommandMinimalEndpoint<in TRequest, in TRequestParameters, TResult1, TResult2, TResult3,
        TResult4> : IMinimalEndpoint
    where TRequestParameters : IHttpCommand<TRequest>
    where TResult1 : IResult
    where TResult2 : IResult
    where TResult3 : IResult
    where TResult4 : IResult
{
    /// <summary>
    /// Asynchronously handles a command request with specified request parameters and returns multiple results.
    /// </summary>
    /// <param name="requestParameters">The request parameters.</param>
    /// <returns>A task that represents the asynchronous operation and returns multiple results.</returns>
    Task<Results<TResult1, TResult2, TResult3, TResult4>> HandleAsync(
        [AsParameters] TRequestParameters requestParameters);
}

/// <summary>
/// Defines the structure for a minimal API endpoint that handles query requests with specified request parameters.
/// </summary>
/// <typeparam name="TRequestParameters">The type of the request parameters.</typeparam>
public interface IQueryMinimalEndpoint<in TRequestParameters> : IMinimalEndpoint
    where TRequestParameters : IHttpQuery
{
    /// <summary>
    /// Asynchronously handles a query request with specified request parameters.
    /// </summary>
    /// <param name="requestParameters">The request parameters.</param>
    /// <returns>A task that represents the asynchronous operation and returns an <see cref="IResult"/>.</returns>
    Task<IResult> HandleAsync([AsParameters] TRequestParameters requestParameters);
}

/// <summary>
/// Defines the structure for a minimal API endpoint that handles query requests with specified request parameters and returns a result.
/// </summary>
/// <typeparam name="TRequestParameters">The type of the request parameters.</typeparam>
/// <typeparam name="TResult1">The type of the first result.</typeparam>
public interface IQueryMinimalEndpoint<in TRequestParameters, TResult1> : IMinimalEndpoint
    where TRequestParameters : IHttpQuery
    where TResult1 : IResult
{
    /// <summary>
    /// Asynchronously handles a query request with specified request parameters and returns a result.
    /// </summary>
    /// <param name="requestParameters">The request parameters.</param>
    /// <returns>A task that represents the asynchronous operation and returns a result of type <typeparamref name="TResult1"/>.</returns>
    Task<TResult1> HandleAsync([AsParameters] TRequestParameters requestParameters);
}

/// <summary>
/// Defines the structure for a minimal API endpoint that handles query requests with specified request parameters and returns multiple results.
/// </summary>
/// <typeparam name="TRequestParameters">The type of the request parameters.</typeparam>
/// <typeparam name="TResult1">The type of the first result.</typeparam>
/// <typeparam name="TResult2">The type of the second result.</typeparam>
public interface IQueryMinimalEndpoint<in TRequestParameters, TResult1, TResult2> : IMinimalEndpoint
    where TRequestParameters : IHttpQuery
    where TResult1 : IResult
    where TResult2 : IResult
{
    /// <summary>
    /// Asynchronously handles a query request with specified request parameters and returns multiple results.
    /// </summary>
    /// <param name="requestParameters">The request parameters.</param>
    /// <returns>A task that represents the asynchronous operation and returns multiple results.</returns>
    Task<Results<TResult1, TResult2>> HandleAsync([AsParameters] TRequestParameters requestParameters);
}

/// <summary>
/// Defines the structure for a minimal API endpoint that handles query requests with specified request parameters and returns multiple results.
/// </summary>
/// <typeparam name="TRequestParameters">The type of the request parameters.</typeparam>
/// <typeparam name="TResult1">The type of the first result.</typeparam>
/// <typeparam name="TResult2">The type of the second result.</typeparam>
/// <typeparam name="TResult3">The type of the third result.</typeparam>
public interface IQueryMinimalEndpoint<in TRequestParameters, TResult1, TResult2, TResult3> : IMinimalEndpoint
    where TRequestParameters : IHttpQuery
    where TResult1 : IResult
    where TResult2 : IResult
    where TResult3 : IResult
{
    /// <summary>
    /// Asynchronously handles a query request with specified request parameters and returns multiple results.
    /// </summary>
    /// <param name="requestParameters">The request parameters.</param>
    /// <returns>A task that represents the asynchronous operation and returns multiple results.</returns>
    Task<Results<TResult1, TResult2, TResult3>> HandleAsync([AsParameters] TRequestParameters requestParameters);
}

/// <summary>
/// Defines the structure for a minimal API endpoint that handles query requests with specified request parameters and returns multiple results.
/// </summary>
/// <typeparam name="TRequestParameters">The type of the request parameters.</typeparam>
/// <typeparam name="TResult1">The type of the first result.</typeparam>
/// <typeparam name="TResult2">The type of the second result.</typeparam>
/// <typeparam name="TResult3">The type of the third result.</typeparam>
/// <typeparam name="TResult4">The type of the fourth result.</typeparam>
public interface IQueryMinimalEndpoint<in TRequestParameters, TResult1, TResult2, TResult3, TResult4> : IMinimalEndpoint
    where TRequestParameters : IHttpQuery
    where TResult1 : IResult
    where TResult2 : IResult
    where TResult3 : IResult
    where TResult4 : IResult
{
    /// <summary>
    /// Asynchronously handles a query request with specified request parameters and returns multiple results.
    /// </summary>
    /// <param name="requestParameters">The request parameters.</param>
    /// <returns>A task that represents the asynchronous operation and returns multiple results.</returns>
    Task<Results<TResult1, TResult2, TResult3, TResult4>> HandleAsync(
        [AsParameters] TRequestParameters requestParameters);
}