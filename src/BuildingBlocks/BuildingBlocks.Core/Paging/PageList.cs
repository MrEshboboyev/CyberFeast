using AutoMapper;
using BuildingBlocks.Abstractions.Core.Paging;

namespace BuildingBlocks.Core.Paging;

/// <summary>
/// Represents a paginated list of items.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
public record PageList<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount) : IPageList<T> where T : class
{
    /// <summary>
    /// Gets the number of items in the current page.
    /// </summary>
    public int CurrentPageSize => Items.Count;

    /// <summary>
    /// Gets the start index of the current page.
    /// </summary>
    public int CurrentStartIndex => TotalCount == 0
        ? 0
        : ((PageNumber - 1) * PageSize) + 1;

    /// <summary>
    /// Gets the end index of the current page.
    /// </summary>
    public int CurrentEndIndex => TotalCount == 0
        ? 0
        : CurrentStartIndex + CurrentPageSize - 1;

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPrevious => PageNumber > 1;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNext => PageNumber < TotalPages;

    /// <summary>
    /// Gets an empty paginated list.
    /// </summary>
    public static PageList<T> Empty => new(
        Enumerable.Empty<T>().ToList(),
        0,
        0,
        0);

    /// <summary>
    /// Creates a new instance of <see cref="PageList{T}"/>.
    /// </summary>
    /// <param name="items">The list of items in the current page.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="totalItems">The total number of items.</param>
    /// <returns>A new instance of <see cref="PageList{T}"/>.</returns>
    public static PageList<T> Create(
        IReadOnlyList<T> items,
        int pageNumber,
        int pageSize, 
        int totalItems)
    {
        return new PageList<T>(items, pageNumber, pageSize, totalItems);
    }

    /// <summary>
    /// Maps the items in the current page to another type using a provided mapping function.
    /// </summary>
    /// <typeparam name="TR">The type to map the items to.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A new instance of <see cref="PageList{TR}"/> with the mapped items.</returns>
    public IPageList<TR> MapTo<TR>(Func<T, TR> map)
        where TR : class
    {
        return PageList<TR>
            .Create(
                Items.Select(map).ToList(),
                PageNumber, 
                PageSize,
                TotalCount);
    }

    /// <summary>
    /// Maps the items in the current page to another type using AutoMapper.
    /// </summary>
    /// <typeparam name="TR">The type to map the items to.</typeparam>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <returns>A new instance of <see cref="PageList{TR}"/> with the mapped items.</returns>
    public IPageList<TR> MapTo<TR>(IMapper mapper)
        where TR : class
    {
        return PageList<TR>.Create(
            mapper.Map<IReadOnlyList<TR>>(Items),
            PageNumber, 
            PageSize,
            TotalCount);
    }
}