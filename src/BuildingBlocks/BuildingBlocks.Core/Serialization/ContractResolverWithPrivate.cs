using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BuildingBlocks.Core.Serialization;

/// <summary>
/// Custom contract resolver that includes properties with private setters.
/// </summary>
internal class ContractResolverWithPrivate : DefaultContractResolver
{
    /// <summary>
    /// Creates a JSON property for the given member and applies custom logic to include private setters.
    /// </summary>
    protected override JsonProperty CreateProperty(
        MemberInfo member,
        MemberSerialization memberSerialization)
    {
        var prop = base.CreateProperty(member, memberSerialization);

        if (prop.Writable) return prop;

        var property = member as PropertyInfo;

        if (property == null) return prop;

        var hasPrivateSetter = property.GetSetMethod(true) != null;
        prop.Writable = hasPrivateSetter;

        return prop;
    }
}