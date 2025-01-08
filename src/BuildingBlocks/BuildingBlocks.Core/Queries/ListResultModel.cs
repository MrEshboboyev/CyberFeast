namespace BuildingBlocks.Core.Queries;

/// <summary>
/// Represents a list of results with pagination information.
/// </summary>
/// <typeparam name="T">The type of the items in the list.</typeparam>
public record ListResultModel<T>(
    IList<T> Items,
    long TotalItems,
    int Page,
    int PageSize)
    where T : notnull
{
    /// <summary>
    /// Gets an empty <see cref="ListResultModel{T}"/>.
    /// </summary>
    public static ListResultModel<T> Empty => new(
        Enumerable.Empty<T>().ToList(),
        0,
        0,
        0);

    /// <summary>
    /// Creates a new instance of <see cref="ListResultModel{T}"/>.
    /// </summary>
    /// <param name="items">The list of items.</param>
    /// <param name="totalItems">The total number of items.</param>
    /// <param name="page">The current page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <returns>A new instance of <see cref="ListResultModel{T}"/>.</returns>
    public static ListResultModel<T> Create(
        IList<T> items,
        long totalItems = 0,
        int page = 1,
        int pageSize = 20)
    {
        return new ListResultModel<T>(items, totalItems, page, pageSize);
    }

    /// <summary>
    /// Maps the items to another type using a provided mapping function.
    /// </summary>
    /// <typeparam name="TU">The type to map the items to.</typeparam>
    /// <param name="map">The mapping function.</param>
    /// <returns>A new instance of <see cref="ListResultModel{TU}"/> with the mapped items.</returns>
    public ListResultModel<TU> Map<TU>(Func<T, TU> map)
        where TU : notnull
    {
        return ListResultModel<TU>.Create(
            Items.Select(map).ToList(),
            TotalItems,
            Page,
            PageSize);
    }
}