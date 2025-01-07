using System.Diagnostics.CodeAnalysis;
using BuildingBlocks.Core.Domain.Exceptions;
using BuildingBlocks.Core.Extensions;

namespace BuildingBlocks.Core.Domain.ValueObjects;

/// <summary>
/// Represents an amount value object.
/// </summary>
public record Amount
{
    // EF
    private Amount(decimal value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the value of the amount.
    /// </summary>
    public decimal Value { get; private set; }

    /// <summary>
    /// Gets an amount with zero value.
    /// </summary>
    public static Amount Zero => Of(0);

    /// <summary>
    /// Creates a new instance of <see cref="Amount"/>.
    /// </summary>
    /// <param name="value">The value of the amount.</param>
    /// <returns>A new instance of <see cref="Amount"/>.</returns>
    public static Amount Of([NotNull] decimal? value)
    {
        value.NotBeNull();

        return Of(value.Value);
    }

    /// <summary>
    /// Creates a new instance of <see cref="Amount"/>.
    /// </summary>
    /// <param name="value">The value of the amount.</param>
    /// <returns>A new instance of <see cref="Amount"/>.</returns>
    /// <exception cref="InvalidAmountException">Thrown if the value is greater than 1,000,000.</exception>
    public static Amount Of([NotNull] decimal value)
    {
        value.NotBeNegativeOrZero();

        if (value > 1000000)
        {
            throw new InvalidAmountException(value);
        }

        return new Amount(value);
    }

    /// <summary>
    /// Implicitly converts an <see cref="Amount"/> to a <see cref="decimal"/>.
    /// </summary>
    /// <param name="value">The amount to convert.</param>
    /// <returns>The amount value as a decimal.</returns>
    public static implicit operator decimal(Amount? value) => value?.Value ?? 0;

    /// <summary>
    /// Compares two <see cref="Amount"/> instances for greater-than relation.
    /// </summary>
    /// <param name="a">The first amount.</param>
    /// <param name="b">The second amount.</param>
    /// <returns><c>true</c> if <c>a</c> is greater than <c>b</c>; otherwise, <c>false</c>.</returns>
    public static bool operator >(Amount a, Amount b) => a.Value > b.Value;

    /// <summary>
    /// Compares two <see cref="Amount"/> instances for less-than relation.
    /// </summary>
    /// <param name="a">The first amount.</param>
    /// <param name="b">The second amount.</param>
    /// <returns><c>true</c> if <c>a</c> is less than <c>b</c>; otherwise, <c>false</c>.</returns>
    public static bool operator <(Amount a, Amount b) => a.Value < b.Value;

    /// <summary>
    /// Compares two <see cref="Amount"/> instances for greater-than-or-equal relation.
    /// </summary>
    /// <param name="a">The first amount.</param>
    /// <param name="b">The second amount.</param>
    /// <returns><c>true</c> if <c>a</c> is greater than or equal to <c>b</c>; otherwise, <c>false</c>.</returns>
    public static bool operator >=(Amount a, Amount b) => a.Value >= b.Value;

    /// <summary>
    /// Compares two <see cref="Amount"/> instances for less-than-or-equal relation.
    /// </summary>
    /// <param name="a">The first amount.</param>
    /// <param name="b">The second amount.</param>
    /// <returns><c>true</c> if <c>a</c> is less than or equal to <c>b</c>; otherwise, <c>false</c>.</returns>
    public static bool operator <=(Amount a, Amount b) => a.Value <= b.Value;

    /// <summary>
    /// Adds two <see cref="Amount"/> instances.
    /// </summary>
    /// <param name="a">The first amount.</param>
    /// <param name="b">The second amount.</param>
    /// <returns>The sum of the two amounts.</returns>
    public static Amount operator +(Amount a, Amount b) => new(a.Value + b.Value);

    /// <summary>
    /// Subtracts one <see cref="Amount"/> instance from another.
    /// </summary>
    /// <param name="a">The first amount.</param>
    /// <param name="b">The second amount.</param>
    /// <returns>The difference between the two amounts.</returns>
    public static Amount operator -(Amount a, Amount b) => new(a.Value - b.Value);

    /// <summary>
    /// Deconstructs the amount into its value.
    /// </summary>
    /// <param name="value">The value of the amount.</param>
    public void Deconstruct(out decimal value) => value = Value;
}