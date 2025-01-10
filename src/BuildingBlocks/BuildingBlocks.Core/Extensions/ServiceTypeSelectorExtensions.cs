using Scrutor;

namespace BuildingBlocks.Core.Extensions;

/// <summary>
/// Provides extension methods for the IServiceTypeSelector interface to register services as closed generic types.
/// </summary>
public static class ServiceTypeSelectorExtensions
{
    /// <summary>
    /// Registers services as closed generic types of the specified closedType.
    /// </summary>
    /// <param name="selector">The selector instance.</param>
    /// <param name="closedType">The closed generic type to register.</param>
    /// <returns>The ILifetimeSelector for chaining further registration methods.</returns>
    public static ILifetimeSelector AsClosedTypeOf(
        this IServiceTypeSelector selector,
        Type closedType)
    {
        return _ = selector.As(t =>
        {
            var types = t.GetInterfaces()
                .Where(p => p.IsGenericType 
                            && p.GetGenericTypeDefinition() == closedType)
                .Select(implementedInterface =>
                    implementedInterface.GenericTypeArguments.Any(a => a.IsTypeDefinition)
                        ? implementedInterface
                        : implementedInterface.GetGenericTypeDefinition()
                )
                .Distinct();
            var result = types.ToList();
            return result;
        });
    }
}