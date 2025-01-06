using BuildingBlocks.Core.Exception.Types;

namespace BuildingBlocks.Core.Domain.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an invalid amount is encountered.
/// </summary>
/// <param name="amount">The invalid amount.</param>
public class InvalidAmountException(
    decimal amount) : BadRequestException($"Amount: '{amount}' is invalid.")
{
    /// <summary>
    /// Gets the invalid amount.
    /// </summary>
    public decimal Amount { get; } = amount;
}