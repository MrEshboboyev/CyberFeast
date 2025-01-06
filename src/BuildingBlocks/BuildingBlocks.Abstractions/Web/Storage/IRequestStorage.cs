namespace BuildingBlocks.Abstractions.Web.Storage;

/// <summary>
/// Defines methods for storing and retrieving request-specific data using key-value pairs.
/// </summary>
public interface IRequestStorage
{
    /// <summary>
    /// Stores a value with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key associated with the value.</param>
    /// <param name="value">The value to store.</param>
    void Set<T>(string key, T value)
        where T : notnull;

    /// <summary>
    /// Retrieves a value by its key.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key associated with the value.</param>
    /// <returns>The value associated with the specified key, or <c>null</c> if not found.</returns>
    T? Get<T>(string key);
}