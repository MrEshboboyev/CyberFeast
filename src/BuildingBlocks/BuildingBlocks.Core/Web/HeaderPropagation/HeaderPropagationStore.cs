using Microsoft.Extensions.Primitives;

namespace BuildingBlocks.Core.Web.HeaderPropagation;

/// <summary>
/// Represents a store for propagating headers across different parts of an application.
/// Values on an instance of <see cref="HeaderPropagationStore"/> will be unique per HTTP async request.
/// </summary>
public class HeaderPropagationStore
{
    private static readonly AsyncLocal<IDictionary<string, StringValues>?> _headers = new();

    /// <summary>
    /// Gets or sets the headers for the current HTTP request.
    /// </summary>
    public IDictionary<string, StringValues> Headers
    {
        get { return _headers.Value ??= new Dictionary<string, StringValues>(); }
        set => _headers.Value = value;
    }
}