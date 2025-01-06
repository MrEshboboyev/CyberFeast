using BuildingBlocks.Core.Exception.Types;

namespace BuildingBlocks.Core.Domain.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an invalid date is encountered.
/// </summary>
/// <param name="date">The invalid date.</param>
public class InvalidDateException(
    DateTime date) : BadRequestException($"Date: '{date}' is invalid.")
{
    /// <summary>
    /// Gets the invalid date.
    /// </summary>
    public DateTime Date { get; } = date;
}