using System.ComponentModel;

namespace BuildingBlocks.Core.Types.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Enum"/>.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the custom attribute of the specified type from the enum value.
    /// </summary>
    /// <typeparam name="T">The type of the attribute.</typeparam>
    /// <param name="value">The enum value.</param>
    /// <returns>The custom attribute of the specified type, or <c>null</c> if the attribute is not found.</returns>
    public static T? GetAttribute<T>(this Enum value)
        where T : Attribute
    {
        var type = value.GetType();
        var memberInfo = type.GetMember(value.ToString());
        var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
        return attributes.Length > 0 ? (T)attributes[0] : null;
    }

    /// <summary>
    /// Gets the description of the enum value from the <see cref="DescriptionAttribute"/>, or the enum value as a string if the attribute is not found.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <returns>The description of the enum value, or the enum value as a string if the description is not found.</returns>
    public static string ToName(this Enum value)
    {
        var attribute = value.GetAttribute<DescriptionAttribute>();
        return attribute == null ? value.ToString() : attribute.Description;
    }
}