using System.Diagnostics.CodeAnalysis;
using BuildingBlocks.Core.Domain.Exceptions;
using BuildingBlocks.Core.Extensions;

namespace BuildingBlocks.Core.Domain.ValueObjects;

/// <summary>
/// Represents a birthdate value object.
/// </summary>
public record BirthDate
{
    // EF Core
    private BirthDate()
    {
    }

    /// <summary>
    /// Gets the value of the birthdate.
    /// </summary>
    public DateTime Value { get; private set; }

    /// <summary>
    /// Creates a new instance of <see cref="BirthDate"/>.
    /// </summary>
    /// <param name="value">The value of the birthdate.</param>
    /// <returns>A new instance of <see cref="BirthDate"/>.</returns>
    public static BirthDate Of([NotNull] DateTime? value)
    {
        value.NotBeNull();

        return Of(value.Value);
    }

    /// <summary>
    /// Creates a new instance of <see cref="BirthDate"/>.
    /// </summary>
    /// <param name="value">The value of the birthdate.</param>
    /// <returns>A new instance of <see cref="BirthDate"/>.</returns>
    /// <exception cref="DomainException">Thrown if the birthdate is invalid.</exception>
    public static BirthDate Of(DateTime value)
    {
        if (value == default)
        {
            throw new DomainException($"BirthDate {value} cannot be null");
        }

        var minDateOfBirth = DateTime.Now.AddYears(-115);
        var maxDateOfBirth = DateTime.Now.AddYears(-15);

        // Validate the minimum age.
        if (value < minDateOfBirth || value > maxDateOfBirth)
        {
            throw new DomainException("The minimum age has to be 15 years.");
        }

        return new BirthDate { Value = value };
    }

    /// <summary>
    /// Implicitly converts a <see cref="BirthDate"/> to a <see cref="DateTime"/>.
    /// </summary>
    /// <param name="value">The birthdate to convert.</param>
    /// <returns>The birthdate value as a DateTime.</returns>
    public static implicit operator DateTime(BirthDate? value) => value?.Value ?? default;

    /// <summary>
    /// Deconstructs the birthdate into its value.
    /// </summary>
    /// <param name="value">The value of the birthdate.</param>
    public void Deconstruct(out DateTime value) => value = Value;
}