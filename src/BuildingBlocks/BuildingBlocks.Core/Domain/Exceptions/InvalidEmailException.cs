using BuildingBlocks.Core.Exception.Types;

namespace BuildingBlocks.Core.Domain.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an invalid email address is encountered.
/// </summary>
/// <param name="email">The invalid email address.</param>
public class InvalidEmailException(
    string email) : BadRequestException($"Email: '{email}' is invalid.")
{
    /// <summary>
    /// Gets the invalid email address.
    /// </summary>
    public string Email { get; } = email;
}