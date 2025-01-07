using System.Diagnostics.CodeAnalysis;
using BuildingBlocks.Core.Extensions;

namespace BuildingBlocks.Core.Domain.ValueObjects;

/// <summary>
/// Represents an address value object.
/// </summary>
public record Address
{
    // EF
    private Address()
    {
    }

    /// <summary>
    /// Gets the country of the address.
    /// </summary>
    public string Country { get; private set; } = null!;

    /// <summary>
    /// Gets the city of the address.
    /// </summary>
    public string City { get; private set; } = null!;

    /// <summary>
    /// Gets the detailed information about the address.
    /// </summary>
    public string Detail { get; private set; } = null!;

    /// <summary>
    /// Gets the postal code of the address.
    /// </summary>
    public PostalCode PostalCode { get; private set; } = null!;

    /// <summary>
    /// Returns an empty address.
    /// </summary>
    public static Address Empty => new();

    /// <summary>
    /// Creates a new instance of <see cref="Address"/>.
    /// </summary>
    /// <param name="country">The country of the address.</param>
    /// <param name="city">The city of the address.</param>
    /// <param name="detail">The detailed information about the address.</param>
    /// <param name="postalCode">The postal code of the address.</param>
    /// <returns>A new instance of <see cref="Address"/>.</returns>
    public static Address Of(
        [NotNull] string? country,
        [NotNull] string? city,
        [NotNull] string? detail,
        [NotNull] PostalCode? postalCode)
    {
        var address = new Address
        {
            Country = country.NotBeNullOrWhiteSpace(),
            City = city.NotBeNullOrWhiteSpace(),
            Detail = detail.NotBeNullOrWhiteSpace(),
            PostalCode = postalCode.NotBeNull()
        };

        return address;
    }

    /// <summary>
    /// Deconstructs the address into its components.
    /// </summary>
    /// <param name="country">The country of the address.</param>
    /// <param name="city">The city of the address.</param>
    /// <param name="detail">The detailed information about the address.</param>
    /// <param name="postalCode">The postal code of the address.</param>
    public void Deconstruct(
        out string country,
        out string city,
        out string detail,
        out PostalCode postalCode) =>
        (country, city, detail, postalCode) = (Country, City, Detail, PostalCode);
}

/// <summary>
/// Represents a postal code value object.
/// </summary>
public record PostalCode
{
    // EF
    // because it is public we don't use value in the parameter, and ef sets value through `Value` property setter
    public PostalCode()
    {
    }

    /// <summary>
    /// Gets the value of the postal code.
    /// </summary>
    public string Value { get; init; } = null!;

    /// <summary>
    /// Creates a new instance of <see cref="PostalCode"/>.
    /// </summary>
    /// <param name="postalCode">The postal code value.</param>
    /// <returns>A new instance of <see cref="PostalCode"/>.</returns>
    public static PostalCode Of(string? postalCode) => new()
    {
        Value = postalCode.NotBeNullOrWhiteSpace()
    };

    /// <summary>
    /// Implicitly converts a <see cref="PostalCode"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="postalCode">The postal code to convert.</param>
    /// <returns>The postal code value as a string.</returns>
    public static implicit operator string(PostalCode? postalCode) =>
        postalCode?.Value ?? string.Empty;
}