namespace BuildingBlocks.Abstractions.Domain;

/// <summary>
/// Represents an aggregate identifier with a generic type.
/// </summary>
/// <typeparam name="T">The type of the identifier.</typeparam>
public record AggregateId<T> : Identity<T>
{
    // EF
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateId{T}"/> class with the specified value.
    /// Protected constructor for Entity Framework.
    /// </summary>
    /// <param name="value">The identifier value.</param>
    protected AggregateId(T value)
    {
        Value = value;
    }

    /// <summary>
    /// Implicitly converts the AggregateId to its identifier type.
    /// </summary>
    /// <param name="id">The AggregateId instance.</param>
    /// <returns>The identifier value.</returns>
    public static implicit operator T(AggregateId<T> id)
    {
        return id.Value;
    }

    /// <summary>
    /// Creates a new instance of AggregateId with the specified identifier value.
    /// Validations should be placed here instead of the constructor.
    /// </summary>
    /// <param name="id">The identifier value.</param>
    /// <returns>A new instance of AggregateId.</returns>
    public static AggregateId<T> CreateAggregateId(T id)
    {
        return new AggregateId<T>(id);
    }
}

/// <summary>
/// Represents an aggregate identifier with a long type.
/// </summary>
public record AggregateId : AggregateId<long>
{
    // EF
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateId"/> class with the specified value.
    /// Protected constructor for Entity Framework.
    /// </summary>
    /// <param name="value">The identifier value.</param>
    protected AggregateId(long value)
        : base(value)
    {
    }

    /// <summary>
    /// Creates a new instance of AggregateId with the specified identifier value.
    /// Validations should be placed here instead of the constructor.
    /// </summary>
    /// <param name="value">The identifier value.</param>
    /// <returns>A new instance of AggregateId.</returns>
    public new static AggregateId CreateAggregateId(long value)
    {
        return new AggregateId(value);
    }

    /// <summary>
    /// Implicitly converts the AggregateId to its identifier type.
    /// </summary>
    /// <param name="id">The AggregateId instance.</param>
    /// <returns>The identifier value.</returns>
    public static implicit operator long(AggregateId? id) => id?.Value ?? 0;
}