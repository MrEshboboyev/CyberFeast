using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BuildingBlocks.Web;

/// <summary>
/// Defines an HTTP QUERY method attribute for API actions.
/// </summary>
public class HttpQueryAttribute : HttpMethodAttribute
{
    private static readonly IEnumerable<string> _supportedMethods = ["QUERY"];

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpQueryAttribute"/> class.
    /// </summary>
    public HttpQueryAttribute()
        : base(_supportedMethods)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpQueryAttribute"/> class with the given route template.
    /// </summary>
    /// <param name="template">The route template. May not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if the template is null.</exception>
    public HttpQueryAttribute([StringSyntax("Route")] string template)
        : base(_supportedMethods, template)
    {
        ArgumentNullException.ThrowIfNull(template);
    }
}