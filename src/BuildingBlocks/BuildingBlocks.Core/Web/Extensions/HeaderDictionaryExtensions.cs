using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Web.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="IHeaderDictionary"/> interface to retrieve and manipulate header values.
/// </summary>
public static class HeaderDictionaryExtensions
{
    /// <summary>
    /// Retrieves all values for a given header key.
    /// </summary>
    /// <typeparam name="T">The type of the header values.</typeparam>
    /// <param name="collection">The header collection.</param>
    /// <param name="key">The header key.</param>
    /// <returns>A collection of header values.</returns>
    public static IEnumerable<T> All<T>(
        this IHeaderDictionary collection,
        string key)
    {
        var values = new List<T>();

        if (!collection.TryGetValue(key, out var results)) return values;
        foreach (var s in results)
        {
            try
            {
                var result = (T)Convert.ChangeType(s, typeof(T))!;
                values.Add(result);
            }
            catch
            {
                // conversion failed
                // skip value
            }
        }

        return values;
    }

    /// <summary>
    /// Retrieves the first or last value for a given header key based on the specified option.
    /// </summary>
    /// <typeparam name="T">The type of the header value.</typeparam>
    /// <param name="collection">The header collection.</param>
    /// <param name="key">The header key.</param>
    /// <param name="default">The default value to return if the header is not found.</param>
    /// <param name="option">The option to specify whether to return the first or last value.</param>
    /// <returns>The header value or the default value.</returns>
    public static T? Get<T>(
        this IHeaderDictionary collection,
        string key,
        T? @default = default,
        ParameterPick option = ParameterPick.First)
    {
        var values = All<T>(collection, key).ToList();
        var value = @default;

        if (values.Count != 0)
        {
            value = option switch
            {
                ParameterPick.First => values.FirstOrDefault(),
                ParameterPick.Last => values.LastOrDefault(),
                _ => value
            };
        }

        return value ?? @default;
    }
}

/// <summary>
/// Specifies the option to pick the first or last value.
/// </summary>
public enum ParameterPick
{
    First,
    Last
}