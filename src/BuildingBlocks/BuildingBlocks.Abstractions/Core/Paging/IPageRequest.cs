namespace BuildingBlocks.Abstractions.Core.Paging;

/// <summary>
/// Defines a contract for paginated requests.
/// </summary>
public interface IPageRequest
{
    int PageNumber { get; init; }
    int PageSize { get; init; }
    string? Filters { get; init; }
    string? SortOrder { get; init; }
}