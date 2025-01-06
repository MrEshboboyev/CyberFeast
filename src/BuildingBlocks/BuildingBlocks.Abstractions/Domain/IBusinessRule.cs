namespace BuildingBlocks.Abstractions.Domain;

/// <summary>
/// Defines a contract for business rules.
/// </summary>
public interface IBusinessRule
{
    /// <summary>
    /// Gets the message describing the business rule.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Gets the status of the business rule.
    /// </summary>
    int Status { get; }

    /// <summary>
    /// Method the business rule is broken.
    /// </summary>
    bool IsBroken();
}