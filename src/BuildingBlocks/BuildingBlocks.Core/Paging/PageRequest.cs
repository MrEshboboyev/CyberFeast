using BuildingBlocks.Abstractions.Core.Paging;

namespace BuildingBlocks.Core.Paging;

/// <summary>
/// Represents a request for paginated data.
/// </summary>
public record PageRequest : IPageRequest
{
    /// <summary>
    /// Gets the requested page number.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Gets the requested page size.
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Gets the filters to apply to the data.
    /// </summary>
    public string? Filters { get; init; }

    /// <summary>
    /// Gets the sort order to apply to the data.
    /// </summary>
    public string? SortOrder { get; init; }

    /// <summary>
    /// Deconstructs the page request into its components.
    /// </summary>
    /// <param name="pageNumber">The requested page number.</param>
    /// <param name="pageSize">The requested page size.</param>
    /// <param name="filters">The filters to apply to the data.</param>
    /// <param name="sortOrder">The sort order to apply to the data.</param>
    public void Deconstruct(
        out int pageNumber,
        out int pageSize,
        out string? filters,
        out string? sortOrder) =>
        (pageNumber, pageSize, filters, sortOrder) =
        (PageNumber, PageSize, Filters, SortOrder);
}