using BuildingBlocks.Abstractions.Queries;
using BuildingBlocks.Core.Paging;

namespace BuildingBlocks.Core.Queries;

/// <summary>
/// Represents a paginated query.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public record PageQuery<TResponse> :
    PageRequest,
    IPageQuery<TResponse>
    where TResponse : notnull;