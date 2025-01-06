using BuildingBlocks.Core.Exception.Types;

namespace BuildingBlocks.Core.Domain.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an invalid currency is encountered.
/// </summary>
/// <param name="currency">The invalid currency.</param>
public class InvalidCurrencyException(
    string currency) : BadRequestException($"Currency: '{currency}' is invalid.")
{
    /// <summary>
    /// Gets the invalid currency.
    /// </summary>
    public string Currency { get; } = currency;
}