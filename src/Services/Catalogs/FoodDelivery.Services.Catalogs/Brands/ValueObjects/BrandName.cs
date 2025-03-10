using BuildingBlocks.Core.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace FoodDelivery.Services.Catalogs.Brands.ValueObjects;

public record BrandName
{
    // EF
    private BrandName() { }

    public string Value { get; private set; } = null!;

    public static BrandName Of([NotNull] string? value)
    {
        // validations should be placed here instead of constructor
        value.NotBeNullOrWhiteSpace();

        return new BrandName { Value = value };
    }

    public static implicit operator string(BrandName value) => value.Value;

    public void Deconstruct(out string value) => value = Value;
}