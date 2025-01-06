namespace BuildingBlocks.Abstractions.Messaging;

/// <summary>
/// Provides a way to store and retrieve context items using a dictionary.
/// </summary>
public class ContextItems
{
    private readonly Dictionary<string, object?> _items = new();

    /// <summary>
    /// Adds an item to the dictionary with the specified key and value.
    /// </summary>
    /// <param name="key">The key of the item.</param>
    /// <param name="value">The value of the item.</param>
    /// <returns>The current <see cref="ContextItems"/> instance.</returns>
    public ContextItems AddItem(string key, object? value)
    {
        _items.TryAdd(key, value);
        return this;
    }

    /// <summary>
    /// Retrieves an item from the dictionary by its key and attempts to cast it to the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to cast the item to.</typeparam>
    /// <param name="key">The key of the item.</param>
    /// <returns>The item cast to the specified type, or default if not found or cast fails.</returns>
    public T? TryGetItem<T>(string key)
    {
        if (_items.TryGetValue(key, out var result))
            return result is T type ? type : default;

        return default;
    }
}