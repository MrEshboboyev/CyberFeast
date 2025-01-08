using BuildingBlocks.Abstractions.Queries;

namespace BuildingBlocks.Core.Queries;

/// <summary>
/// Represents a query for a list of results.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public record ListQuery<TResponse> : IListQuery<TResponse>
    where TResponse : notnull
{
    /// <summary>
    /// Gets or sets the list of includes for related data.
    /// </summary>
    public IList<string>? Includes { get; init; }

    /// <summary>
    /// Gets or sets the list of sorts for ordering the data.
    /// </summary>
    public IList<string>? Sorts { get; init; }

    /// <summary>
    /// Gets or sets the requested page number.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Gets or sets the requested page size.
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Gets or sets the filters to apply to the data.
    /// </summary>
    public string? Filters { get; init; }

    /// <summary>
    /// Gets or sets the sort order to apply to the data.
    /// </summary>
    public string? SortOrder { get; init; }
}