using System.Reflection;

namespace BuildingBlocks.Core.Domain;

/// <summary>
/// Represents a base class for creating enumerations.
/// </summary>
public abstract class Enumeration : IComparable
{
    /// <summary>
    /// Gets the name of the enumeration.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the identifier of the enumeration.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Enumeration"/> class.
    /// </summary>
    /// <param name="id">The identifier of the enumeration.</param>
    /// <param name="name">The name of the enumeration.</param>
    protected Enumeration(int id, string name) => (Id, Name) = (id, name);

    /// <summary>
    /// Returns the name of the enumeration.
    /// </summary>
    /// <returns>The name of the enumeration.</returns>
    public override string ToString() => Name;

    /// <summary>
    /// Retrieves all instances of the enumeration type.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <returns>A collection of all instances of the enumeration type.</returns>
    public static IEnumerable<T> GetAll<T>()
        where T : Enumeration =>
        typeof(T)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();

    /// <summary>
    /// Compares this instance to another <see cref="Enumeration"/> instance for equality.
    /// </summary>
    /// <param name="obj">The other enumeration instance.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    /// <summary>
    /// Returns the hash code for the enumeration.
    /// </summary>
    /// <returns>The hash code for the enumeration.</returns>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Calculates the absolute difference between the IDs of two enumerations.
    /// </summary>
    /// <param name="firstValue">The first enumeration instance.</param>
    /// <param name="secondValue">The second enumeration instance.</param>
    /// <returns>The absolute difference between the IDs of the two enumerations.</returns>
    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
        return absoluteDifference;
    }

    /// <summary>
    /// Retrieves an instance of the enumeration type by its ID.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="value">The ID of the enumeration.</param>
    /// <returns>The matching enumeration instance.</returns>
    public static T FromValue<T>(int value)
        where T : Enumeration
    {
        var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
        return matchingItem;
    }

    /// <summary>
    /// Retrieves an instance of the enumeration type by its display name.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="displayName">The display name of the enumeration.</param>
    /// <returns>The matching enumeration instance.</returns>
    public static T FromDisplayName<T>(string displayName)
        where T : Enumeration
    {
        var matchingItem = Parse<T, string>(displayName, 
            "display name", item => item.Name == displayName);
        return matchingItem;
    }

    private static T Parse<T, TK>(TK value, string description, Func<T, bool> predicate)
        where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem == null)
            throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

        return matchingItem;
    }

    /// <summary>
    /// Compares this instance to another <see cref="Enumeration"/> instance by their IDs.
    /// </summary>
    /// <param name="other">The other enumeration instance.</param>
    /// <returns>An integer that indicates the relative order of the instances being compared.</returns>
    public int CompareTo(object? other) => Id.CompareTo(((Enumeration)other!).Id);
}