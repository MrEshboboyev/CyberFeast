using Xunit.Sdk;

namespace Tests.Shared.XunitCategories;

/// <summary>
/// Could filter by 'dotnet test --filter "Category=TestCategory"' in running tests in command line
/// </summary>
[TraitDiscoverer(CategoryTraitDiscoverer.DiscovererTypeName, XunitConstants.AssemblyName)]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class CategoryTraitAttribute(TestCategory category) : Attribute, ITraitAttribute
{
    public TestCategory Name { get; } = category;
}

public class CategoryTraitDiscoverer : ITraitDiscoverer
{
    private const string Key = "Category";

    public const string DiscovererTypeName =
        $"{XunitConstants.AssemblyName}.{nameof(XunitCategories)}.{nameof(CategoryTraitDiscoverer)}";

    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        var categoryName = traitAttribute.GetNamedArgument<TestCategory?>("Name");

        if (categoryName is not null)
            yield return new KeyValuePair<string, string>(Key, categoryName.ToString()!);
    }
}

public enum TestCategory
{
    Unit,
    Integration,
    EndToEnd,
    LoadTest,
    SkipCi
}
