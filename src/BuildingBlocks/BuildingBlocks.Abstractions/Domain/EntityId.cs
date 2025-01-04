namespace BuildingBlocks.Abstractions.Domain;

/// <summary>
/// Represents an identifier for an entity with a generic type.
/// </summary>
/// <typeparam name="T">The type of the identifier.</typeparam>
public record EntityId<T> : Identity<T>
{
    /// <summary>
    /// Implicitly converts the EntityId to its identifier type.
    /// </summary>
    /// <param name="id">The EntityId instance.</param>
    /// <returns>The identifier value.</returns>
    public static implicit operator T(EntityId<T> id)
    {
        ArgumentNullException.ThrowIfNull(id.Value);
        return id.Value;
    }

    /// <summary>
    /// Creates a new instance of EntityId with the specified identifier value.
    /// </summary>
    /// <param name="id">The identifier value.</param>
    /// <returns>A new instance of EntityId.</returns>
    public static EntityId<T> Of(T id)
    {
        return new EntityId<T> { Value = id };
    }
}

/// <summary>
/// Represents an identifier for an entity with a long type.
/// </summary>
public record EntityId : EntityId<long>
{
    /// <summary>
    /// Implicitly converts the EntityId to its identifier type.
    /// </summary>
    /// <param name="id">The EntityId instance.</param>
    /// <returns>The identifier value.</returns>
    public static implicit operator long(EntityId id)
    {
        return id.Value;
    }

    /// <summary>
    /// Creates a new instance of EntityId with the specified identifier value.
    /// </summary>
    /// <param name="id">The identifier value.</param>
    /// <returns>A new instance of EntityId.</returns>
    public new static EntityId Of(long id)
    {
        return new EntityId { Value = id };
    }
}