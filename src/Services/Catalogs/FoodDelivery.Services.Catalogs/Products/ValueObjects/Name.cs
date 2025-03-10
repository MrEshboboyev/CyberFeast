using System.Diagnostics.CodeAnalysis;
using BuildingBlocks.Core.Extensions;

namespace FoodDelivery.Services.Catalogs.Products.ValueObjects;

public record Name
{
    // EF
    private Name() { }

    public string Value { get; private set; } = null!;

    public static Name Of([NotNull] string? value)
    {
        // validations should be placed here instead of constructor
        value.NotBeNullOrWhiteSpace();

        return new Name { Value = value };
    }

    public static implicit operator string(Name value) => value.Value;

    public void Deconstruct(out string value) => value = Value;
}