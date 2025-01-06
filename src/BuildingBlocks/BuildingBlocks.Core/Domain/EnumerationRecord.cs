using System.Reflection;

namespace BuildingBlocks.Core.Domain;

/// <summary>
/// Represents a base class for creating enumeration records.
/// </summary>
/// <typeparam name="T">The type of the enumeration record.</typeparam>
/// <param name="Value">The value of the enumeration record.</param>
/// <param name="DisplayName">The display name of the enumeration record.</param>
public abstract record EnumerationRecord<T>(int Value, string DisplayName) : IComparable<T>
    where T : EnumerationRecord<T>
{
    private static readonly Lazy<Dictionary<int, T>> _allItems;
    private static readonly Lazy<Dictionary<string, T>> _allItemsByName;

    static EnumerationRecord()
    {
        _allItems = new Lazy<Dictionary<int, T>>(() =>
        {
            return typeof(T)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(x => x.FieldType == typeof(T))
                .Select(x => x.GetValue(null))
                .Cast<T>()
                .ToDictionary(x => x.Value, x => x);
        });
        _allItemsByName = new Lazy<Dictionary<string, T>>(() =>
        {
            var items = new Dictionary<string, T>(_allItems.Value.Count);
            foreach (var item in _allItems.Value)
            {
                if (!items.TryAdd(item.Value.DisplayName, item.Value))
                {
                    throw new Exception(
                        $"DisplayName needs to be unique. '{item.Value.DisplayName}' already exists"
                    );
                }
            }

            return items;
        });
    }

    /// <summary>
    /// Returns the display name of the enumeration record.
    /// </summary>
    /// <returns>The display name of the enumeration record.</returns>
    public override string ToString() => DisplayName;

    /// <summary>
    /// Retrieves all instances of the enumeration record type.
    /// </summary>
    /// <returns>A collection of all instances of the enumeration record type.</returns>
    public static IEnumerable<T> GetAll()
    {
        return _allItems.Value.Values;
    }

    /// <summary>
    /// Calculates the absolute difference between the values of two enumeration records.
    /// </summary>
    /// <param name="firstValue">The first enumeration record instance.</param>
    /// <param name="secondValue">The second enumeration record instance.</param>
    /// <returns>The absolute difference between the values of the two enumeration records.</returns>
    public static int AbsoluteDifference(
        EnumerationRecord<T> firstValue, 
        EnumerationRecord<T> secondValue)
    {
        return Math.Abs(firstValue.Value - secondValue.Value);
    }

    /// <summary>
    /// Retrieves an instance of the enumeration record type by its value.
    /// </summary>
    /// <param name="value">The value of the enumeration record.</param>
    /// <returns>The matching enumeration record instance.</returns>
    public static T FromValue(int value)
    {
        if (_allItems.Value.TryGetValue(value, out var matchingItem))
        {
            return matchingItem;
        }

        throw new InvalidOperationException($"'{value}' is not a valid value in {typeof(T)}");
    }

    /// <summary>
    /// Retrieves an instance of the enumeration record type by its display name.
    /// </summary>
    /// <param name="displayName">The display name of the enumeration record.</param>
    /// <returns>The matching enumeration record instance.</returns>
    public static T FromDisplayName(string displayName)
    {
        if (_allItemsByName.Value.TryGetValue(displayName, out var matchingItem))
        {
            return matchingItem;
        }

        throw new InvalidOperationException($"'{displayName}' is not a valid display name in {typeof(T)}");
    }

    /// <summary>
    /// Compares this instance to another <see cref="EnumerationRecord{T}"/> instance by their values.
    /// </summary>
    /// <param name="other">The other enumeration record instance.</param>
    /// <returns>An integer that indicates the relative order of the instances being compared.</returns>
    public int CompareTo(T? other) => Value.CompareTo(other!.Value);
}