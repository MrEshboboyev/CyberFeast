using System.Collections.Concurrent;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Reflection;

namespace BuildingBlocks.Core.Types;

/// <summary>
/// Provides methods for mapping types to and from their names.
/// </summary>
public static class TypeMapper
{
    private static readonly ConcurrentDictionary<Type, string> TypeNameMap = new();
    private static readonly ConcurrentDictionary<string, Type> TypeMap = new();

    /// <summary>
    /// Gets the full type name from a generic Type class.
    /// </summary>
    /// <typeparam name="T">The type to get the name of.</typeparam>
    /// <returns>The full type name.</returns>
    public static string GetFullTypeName<T>() => ToName(typeof(T));

    /// <summary>
    /// Gets the type name from a generic Type class without namespace.
    /// </summary>
    /// <typeparam name="T">The type to get the name of.</typeparam>
    /// <returns>The type name without namespace.</returns>
    public static string GetTypeName<T>() => ToName(typeof(T), false);

    /// <summary>
    /// Gets the type name from a Type class without namespace.
    /// </summary>
    /// <param name="type">The type to get the name of.</param>
    /// <returns>The type name without namespace.</returns>
    public static string GetTypeName(Type type) => ToName(type, false);

    /// <summary>
    /// Gets the full type name from a Type class.
    /// </summary>
    /// <param name="type">The type to get the name of.</param>
    /// <returns>The full type name.</returns>
    public static string GetFullTypeName(Type type) => ToName(type);

    /// <summary>
    /// Gets the type name from an instance object without namespace.
    /// </summary>
    /// <param name="o">The object instance to get the type name of.</param>
    /// <returns>The type name without namespace.</returns>
    public static string GetTypeNameByObject(object o) => ToName(o.GetType(), false);

    /// <summary>
    /// Gets the full type name from an instance object.
    /// </summary>
    /// <param name="o">The object instance to get the full type name of.</param>
    /// <returns>The full type name.</returns>
    public static string GetFullTypeNameByObject(object o) => ToName(o.GetType());

    /// <summary>
    /// Gets the type class from a type name.
    /// </summary>
    /// <param name="typeName">The type name.</param>
    /// <returns>The type class.</returns>
    public static Type GetType(string typeName) => ToType(typeName);

    /// <summary>
    /// Adds a type to the type map.
    /// </summary>
    /// <typeparam name="T">The type to add.</typeparam>
    /// <param name="name">The name to associate with the type.</param>
    public static void AddType<T>(string name) => AddType(typeof(T), name);

    /// <summary>
    /// Adds a type to the type map.
    /// </summary>
    /// <param name="type">The type to add.</param>
    /// <param name="name">The name to associate with the type.</param>
    private static void AddType(Type type, string name)
    {
        ToName(type);
        ToType(name);
    }

    /// <summary>
    /// Checks if a type is registered in the type map.
    /// </summary>
    /// <typeparam name="T">The type to check.</typeparam>
    /// <returns><c>true</c> if the type is registered; otherwise, <c>false</c>.</returns>
    public static bool IsTypeRegistered<T>() => TypeNameMap.ContainsKey(typeof(T));

    /// <summary>
    /// Converts a type to its name and adds it to the type map.
    /// </summary>
    /// <param name="type">The type to convert.</param>
    /// <param name="fullName">Whether to use the full name.</param>
    /// <returns>The type name.</returns>
    private static string ToName(Type type, bool fullName = true)
    {
        type.NotBeNull();
        return TypeNameMap.GetOrAdd(type, _ =>
        {
            var eventTypeName = fullName ? type.FullName! : type.Name;
            TypeMap.GetOrAdd(eventTypeName, type);
            return eventTypeName;
        });
    }

    /// <summary>
    /// Converts a type name to its type and adds it to the type map.
    /// </summary>
    /// <param name="typeName">The type name to convert.</param>
    /// <returns>The type.</returns>
    private static Type ToType(string? typeName)
    {
        typeName.NotBeNull();
        return TypeMap.GetOrAdd(typeName, _ =>
        {
            var type = ReflectionUtilities
                .GetFirstMatchingTypeFromCurrentDomainAssemblies(typeName);
            if (type == null)
                throw new System.Exception($"Type map for '{typeName}' wasn't found!");
            return type;
        });
    }
}