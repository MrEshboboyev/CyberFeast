using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Core.Web.Extensions;

/// <summary>
/// Provides extension methods for mapping query endpoints.
/// </summary>
public static class HttpQueryExtensions
{
    /// <summary>
    /// Maps a query endpoint using a specified pattern and request delegate for handling the query.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <param name="pattern">The pattern to match for the query endpoint.</param>
    /// <param name="requestDelegate">The request delegate for handling the query.</param>
    /// <returns>An endpoint convention builder for the mapped query endpoint.</returns>
    public static IEndpointConventionBuilder MapQuery(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Func<DbLoggerCategory.Query, IResult> requestDelegate)
    {
        return endpoints.MapMethods(pattern, ["QUERY"], requestDelegate);
    }

    /// <summary>
    /// Maps a query endpoint using a specified pattern and request delegate for handling the query.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <param name="pattern">The pattern to match for the query endpoint.</param>
    /// <param name="requestDelegate">The request delegate for handling the query.</param>
    /// <returns>An endpoint convention builder for the mapped query endpoint.</returns>
    public static IEndpointConventionBuilder MapQuery(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        RequestDelegate requestDelegate)
    {
        return endpoints.MapMethods(pattern, ["QUERY"], requestDelegate);
    }
}