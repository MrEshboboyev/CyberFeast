namespace BuildingBlocks.Core.Types.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IDictionary{TKey, TValue}"/>.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Tries to add the specified key and value to the dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary to add the key and value to.</param>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to add.</param>
    /// <returns><c>true</c> if the key and value were added; otherwise, <c>false</c>.</returns>
    public static bool TryAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value)
    {
        if (dictionary.ContainsKey(key))
        {
            return false;
        }

        dictionary.Add(key, value);
        return true;
    }

    /// <summary>
    /// Adds or replaces the specified key and value in the dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary to add or replace the key and value in.</param>
    /// <param name="key">The key to add or replace.</param>
    /// <param name="value">The value to add or replace.</param>
    /// <returns><c>true</c> if the key and value were added or replaced; otherwise, <c>false</c>.</returns>
    public static bool AddOrReplace<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key, TValue value)
    {
        dictionary.Remove(key);

        return dictionary.TryAdd(key, value);
    }

    /// <summary>
    /// Gets the value associated with the specified key from the dictionary.
    /// </summary>
    /// <param name="dictionary">The dictionary to get the value from.</param>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the key, or <c>null</c> if the key does not exist.</returns>
    public static object? Get(
        this IDictionary<string, object?> dictionary,
        string key)
    {
        dictionary.TryGetValue(key, out var val);

        return val;
    }

    /// <summary>
    /// Gets the value associated with the specified key from the dictionary.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to get.</typeparam>
    /// <param name="dictionary">The dictionary to get the value from.</param>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the key, or the default value of <typeparamref name="TValue"/> if the key does not exist.</returns>
    public static TValue? Get<TValue>(
        this IDictionary<string, object?> dictionary,
        string key)
    {
        dictionary.TryGetValue(key, out var val);

        return val is null ? default : (TValue)val;
    }
}