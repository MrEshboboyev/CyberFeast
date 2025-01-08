using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BuildingBlocks.Core.Types.Extensions;

/// <summary>
/// Provides extension methods for <see cref="string"/>.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts the input object to a specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert to.</typeparam>
    /// <param name="input">The input object to convert.</param>
    /// <returns>The converted value.</returns>
    public static T ConvertTo<T>(this object input)
    {
        return ConvertTo<T>(input.ToString());
    }

    /// <summary>
    /// Converts the input string to a specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert to.</typeparam>
    /// <param name="input">The input string to convert.</param>
    /// <returns>The converted value.</returns>
    public static T ConvertTo<T>(this string input)
    {
        try
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFromString(input);
        }
        catch (NotSupportedException)
        {
            return default;
        }
    }

    /// <summary>
    /// Checks if the input string is a valid JSON.
    /// </summary>
    /// <param name="strInput">The input string to check.</param>
    /// <returns><c>true</c> if the input string is a valid JSON; otherwise, <c>false</c>.</returns>
    public static bool IsValidJson(this string strInput)
    {
        if (string.IsNullOrWhiteSpace(strInput))
        {
            return false;
        }

        strInput = strInput.Trim();
        if ((!strInput.StartsWith('{') || !strInput.EndsWith('}')) &&
            (!strInput.StartsWith('[') || !strInput.EndsWith(']')))
        {
            return false;
        }

        try
        {
            var obj = JToken.Parse(strInput);
            return true;
        }
        catch (JsonReaderException jex)
        {
            Console.WriteLine(jex.Message);
            return false;
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }

    /// <summary>
    /// Gets a deterministic hash code for the input string.
    /// </summary>
    /// <param name="str">The input string.</param>
    /// <returns>The deterministic hash code.</returns>
    public static int GetDeterministicHashCode(this string str)
    {
        unchecked
        {
            var hash1 = (5381 << 16) + 5381;
            var hash2 = hash1;

            for (var i = 0; i < str.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }
}