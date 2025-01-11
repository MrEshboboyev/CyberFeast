using System.Reflection;
using System.Runtime.InteropServices;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace BuildingBlocks.Persistence.Mongo;

/// <summary>
/// A convention that maps all read-only properties for which a matching constructor is found.
/// </summary>
public class ImmutablePocoConvention(BindingFlags bindingFlags)
    : ConventionBase, IClassMapConvention
{
    private readonly BindingFlags _bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutablePocoConvention"/> class with the specified binding flags.
    /// </summary>
    public ImmutablePocoConvention()
        : this(BindingFlags.Instance | BindingFlags.Public)
    {
    }

    /// <summary>
    /// Applies the convention to the specified class map.
    /// </summary>
    /// <param name="classMap">The class map.</param>
    public void Apply(BsonClassMap classMap)
    {
        var readOnlyProperties = classMap
            .ClassType.GetTypeInfo()
            .GetProperties(_bindingFlags)
            .Where(p => IsReadOnlyProperty(classMap, p))
            .ToList();

        foreach (var constructor in classMap.ClassType.GetConstructors())
        {
            // If a matching constructor is found, map it and all the read-only properties
            var matchProperties = GetMatchingProperties(constructor, readOnlyProperties);
            if (matchProperties.Count == 0) continue;

            // Map constructor
            classMap.MapConstructor(constructor);

            // Map properties
            foreach (var p in CollectionsMarshal.AsSpan(matchProperties))
                classMap.MapMember(p);
        }
    }

    #region Private Methods

    /// <summary>
    /// Gets the matching properties for the specified constructor.
    /// </summary>
    /// <param name="constructor">The constructor info.</param>
    /// <param name="properties">The list of properties.</param>
    /// <returns>A list of matching properties.</returns>
    private static List<PropertyInfo> GetMatchingProperties(ConstructorInfo constructor, List<PropertyInfo> properties)
    {
        var matchProperties = new List<PropertyInfo>();

        var ctorParameters = constructor.GetParameters();
        foreach (var ctorParameter in ctorParameters)
        {
            var matchProperty = properties.FirstOrDefault(p => ParameterMatchProperty(ctorParameter, p));
            if (matchProperty == null)
                return [];

            matchProperties.Add(matchProperty);
        }

        return matchProperties;
    }

    /// <summary>
    /// Determines whether a constructor parameter matches a property.
    /// </summary>
    /// <param name="parameter">The parameter info.</param>
    /// <param name="property">The property info.</param>
    /// <returns>True if the parameter matches the property, otherwise false.</returns>
    private static bool ParameterMatchProperty(ParameterInfo parameter, PropertyInfo property)
    {
        return
            string.Equals(property.Name, parameter.Name, StringComparison.InvariantCultureIgnoreCase) &&
            parameter.ParameterType == property.PropertyType;
    }

    /// <summary>
    /// Determines whether a property is read-only for the specified class map.
    /// </summary>
    /// <param name="classMap">The class map.</param>
    /// <param name="propertyInfo">The property info.</param>
    /// <returns>True if the property is read-only, otherwise false.</returns>
    private static bool IsReadOnlyProperty(BsonClassMap classMap, PropertyInfo propertyInfo)
    {
        // We can't read the property
        if (!propertyInfo.CanRead)
            return false;

        // We can write to the property (already handled by the default convention)
        if (propertyInfo.CanWrite)
            return false;

        // Skip indexers
        if (propertyInfo.GetIndexParameters().Length != 0)
            return false;

        // Skip overridden properties (already included by the base class)
        var getMethodInfo = propertyInfo.GetMethod;
        return !getMethodInfo.IsVirtual || getMethodInfo.GetBaseDefinition().DeclaringType == classMap.ClassType;
    }

    #endregion
}