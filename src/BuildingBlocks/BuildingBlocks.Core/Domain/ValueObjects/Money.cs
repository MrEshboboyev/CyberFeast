using BuildingBlocks.Core.Extensions;

namespace BuildingBlocks.Core.Domain.ValueObjects;

public record Money
{
    // EF
    private Money() { }

    public decimal Value { get; private set; }
    public string Currency { get; private set; } = default!;

    public static Money Of(decimal? value, string? currency)
    {
        // validations should be placed here instead of constructor
        value.NotBeNull();
        value.NotBeNegativeOrZero();
        currency.NotBeNullOrWhiteSpace();
        currency.NotBeInvalidCurrency();

        return new Money { Currency = currency, Value = value.Value };
    }

    public static Money operator *(int left, Money right)
    {
        return Of(right.Value * left, right.Currency);
    }

    public void Deconstruct(
        out decimal value,
        out string currency) =>
        (value, currency) = (Value, Currency);
}
