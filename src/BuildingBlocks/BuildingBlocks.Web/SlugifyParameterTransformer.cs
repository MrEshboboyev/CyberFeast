using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Web;

/// <summary>
/// Transforms route parameter values into a slugified format.
/// </summary>
public partial class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    /// <summary>
    /// Transforms the parameter value to a slugified string.
    /// </summary>
    /// <param name="value">The parameter value.</param>
    /// <returns>The slugified string.</returns>
    public string? TransformOutbound(object? value)
    {
        return value == null
            ? null
            : SlugifyRegex()
                .Replace(value.ToString() ?? string.Empty, "$1-$2")
                .ToLower();
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex SlugifyRegex();
}