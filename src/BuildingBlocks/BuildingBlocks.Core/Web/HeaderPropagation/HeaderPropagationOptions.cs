using Microsoft.Extensions.Primitives;

namespace BuildingBlocks.Core.Web.HeaderPropagation;

/// <summary>
/// Represents custom options for header propagation.
/// </summary>
public class CustomHeaderPropagationOptions
{
    /// <summary>
    /// Gets or sets the list of header names to propagate.
    /// </summary>
    public IList<string> HeaderNames { get; set; } = new List<string>();
}