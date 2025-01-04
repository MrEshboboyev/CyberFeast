namespace BuildingBlocks.Abstractions.Domain;

/// <summary>
/// Defines a contract for business rules with a specific exception type.
/// </summary>
/// <typeparam name="TException">The type of exception associated with the business rule.</typeparam>
public interface IBusinessRuleWithExceptionType<out TException>
    where TException : Exception
{
    /// <summary>
    /// Gets the exception instance associated with the business rule.
    /// </summary>
    TException Exception { get; }

    /// <summary>
    /// Indicates whether the business rule is broken.
    /// </summary>
    bool IsBroken { get; }
}