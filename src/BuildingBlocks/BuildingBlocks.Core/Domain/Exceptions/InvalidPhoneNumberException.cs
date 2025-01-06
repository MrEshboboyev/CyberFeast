using BuildingBlocks.Core.Exception.Types;

namespace BuildingBlocks.Core.Domain.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an invalid phone number is encountered.
/// </summary>
/// <param name="phoneNumber">The invalid phone number.</param>
public class InvalidPhoneNumberException(string phoneNumber)
    : BadRequestException($"PhoneNumber: '{phoneNumber}' is invalid.")
{
    /// <summary>
    /// Gets the invalid phone number.
    /// </summary>
    public string PhoneNumber { get; } = phoneNumber;
}