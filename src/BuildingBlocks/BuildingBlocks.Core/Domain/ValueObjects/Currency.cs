using System.Diagnostics.CodeAnalysis;
using BuildingBlocks.Core.Extensions;

namespace BuildingBlocks.Core.Domain.ValueObjects;

public record Currency
{
    // EF
    private Currency(string value)
    {
        Value = value;
    }
    
    public string Value { get; private set; } = default!;

    public static Currency Of([NotNull] string? value)
    {
        // validations should be placed here instead of constructor
        value.NotBeNullOrWhiteSpace();
        value.NotBeInvalidCurrency();
        return new Currency(value);
    }

    public static implicit operator string(Currency? value) => value?.Value ?? string.Empty;

    public void Deconstruct(out string value) => value = Value;
}
