using System.Collections;
using BuildingBlocks.Core.Types.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BuildingBlocks.Core.Web.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="IQueryCollection"/> interface to retrieve and manipulate query string values.
/// </summary>
public static class QueryCollectionExtensions
{
    /// <summary>
    /// Retrieves all values for a given query key.
    /// </summary>
    /// <typeparam name="T">The type of the query values.</typeparam>
    /// <param name="collection">The query collection.</param>
    /// <param name="key">The query key.</param>
    /// <returns>A collection of query values.</returns>
    public static IEnumerable<T> All<T>(
        this IQueryCollection collection,
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
            catch (System.Exception)
            {
                // conversion failed
                // skip value
            }
        }

        return values;
    }

    /// <summary>
    /// Retrieves the first or last value for a given query key based on the specified option.
    /// </summary>
    /// <typeparam name="T">The type of the query value.</typeparam>
    /// <param name="collection">The query collection.</param>
    /// <param name="key">The query key.</param>
    /// <param name="default">The default value to return if the query key is not found.</param>
    /// <param name="option">The option to specify whether to return the first or last value.</param>
    /// <returns>The query value or the default value.</returns>
    public static T Get<T>(
        this IQueryCollection collection,
        string key,
        T @default = default,
        ParameterPick option = ParameterPick.First)
    {
        var values = All<T>(collection, key);
        var value = @default;

        if (values.Any())
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

    /// <summary>
    /// Retrieves a collection of values for a given query key, deserializing JSON values if applicable.
    /// </summary>
    /// <typeparam name="T">The type of the collection values.</typeparam>
    /// <param name="collection">The query collection.</param>
    /// <param name="key">The query key.</param>
    /// <param name="default">The default value to return if the query key is not found.</param>
    /// <returns>A collection of values or the default value.</returns>
    public static T GetCollection<T>(
        this IQueryCollection collection,
        string key,
        T @default = default) where T : IEnumerable
    {
        var type = typeof(T).GetGenericArguments()[0];
        var listType = typeof(List<>);
        var constructedListType = listType.MakeGenericType(type);
        dynamic values = Activator.CreateInstance(constructedListType);

        if (collection.TryGetValue(key, out var results))
        {
            foreach (var s in results)
            {
                try
                {
                    if (s.IsValidJson())
                    {
                        dynamic result = JsonConvert.DeserializeObject(s, type);
                        values.Add(result);
                    }
                    else
                    {
                        dynamic result = Convert.ChangeType(s, type);
                        values.Add(result);
                    }
                }
                catch (System.Exception)
                {
                    // conversion failed
                    // skip value
                }
            }
        }
        else
        {
            return @default;
        }

        return values;
    }
}