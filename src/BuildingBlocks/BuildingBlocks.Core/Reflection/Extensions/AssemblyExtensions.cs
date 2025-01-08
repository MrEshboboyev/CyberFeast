using System.Reflection;

namespace BuildingBlocks.Core.Reflection.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Assembly"/> class.
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// Gets all loadable types from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to retrieve types from.</param>
    /// <returns>An enumerable of loadable types.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the assembly is null.</exception>
    public static IEnumerable<Type?> GetLoadableTypes(this Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t != null);
        }
    }
}