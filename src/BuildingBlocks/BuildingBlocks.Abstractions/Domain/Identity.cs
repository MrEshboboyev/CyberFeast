namespace BuildingBlocks.Abstractions.Domain;

/// <summary>
/// Represents an abstract base record for identity types with a generic identifier.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public abstract record Identity<TId>
{
    /// <summary>
    /// Gets or initializes the identifier value.
    /// </summary>
    public TId Value { get; init; } = default!;

    /// <summary>
    /// Converts the Identity instance to its identifier type implicitly.
    /// </summary>
    /// <param name="identityId">The identity instance.</param>
    /// <returns>The identifier value.</returns>
    public static implicit operator TId(Identity<TId> identityId)
    {
        return identityId.Value;
    }

    /// <summary>
    /// Returns a string representation of the identifier.
    /// </summary>
    /// <returns>A string representation of the identifier.</returns>
    public override string ToString()
    {
        return IdAsString();
    }

    /// <summary>
    /// Returns a detailed string representation of the identifier.
    /// </summary>
    /// <returns>A detailed string representation of the identifier.</returns>
    public string IdAsString()
    {
        return $"{GetType().Name} [InternalCommandId={Value}]";
    }
}

/// <summary>
/// Represents an abstract base record for identity types with a long identifier.
/// </summary>
public abstract record Identity : Identity<long>;