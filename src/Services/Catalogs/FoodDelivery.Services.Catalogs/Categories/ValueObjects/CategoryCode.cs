using System.Diagnostics.CodeAnalysis;
using BuildingBlocks.Core.Extensions;

namespace FoodDelivery.Services.Catalogs.Categories.ValueObjects;

public record CategoryCode
{
    // EF
    private CategoryCode() { }

    public string Value { get; private set; } = null!;

    public static CategoryCode Of([NotNull] string? value)
    {
        // validations should be placed here instead of constructor
        value.NotBeNullOrWhiteSpace();

        return new CategoryCode { Value = value };
    }

    public static implicit operator string(CategoryCode value) => value.Value;

    public void Deconstruct(out string value) => value = Value;
}