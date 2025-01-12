using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web;

/// <summary>
/// Represents a query object with a text property.
/// </summary>
public class Query
{
    /// <summary>
    /// Gets or sets the query text.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Binds query data from the HTTP context to a query object.
    /// </summary>
    /// <param name="context">The HTTP context containing the query data.</param>
    /// <param name="parameter">The parameter information.</param>
    /// <returns>A <see cref="Query"/> object with the bound data.</returns>
    public static async ValueTask<Query> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        string? text = null;
        var request = context.Request;

        // Enable buffering if the request body is not seekable
        if (!request.Body.CanSeek)
        {
            request.EnableBuffering();
        }

        // Read the request body if it is readable
        if (!request.Body.CanRead) return new Query { Text = text };
        
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8);
        text = await reader.ReadToEndAsync().ConfigureAwait(false);
        request.Body.Position = 0;

        return new Query
        {
            Text = text
        };
    }

    /// <summary>
    /// Implicitly converts a <see cref="Query"/> object to a string.
    /// </summary>
    /// <param name="query">The query object.</param>
    /// <returns>The query text.</returns>
    public static implicit operator string(Query query)
    {
        return query.Text ?? string.Empty;
    }
}