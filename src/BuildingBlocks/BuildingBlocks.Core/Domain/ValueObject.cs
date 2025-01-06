namespace BuildingBlocks.Core.Domain;

/// <summary>
/// Represents a value object in the domain-driven design (DDD) pattern.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Gets the components used to determine equality.
    /// </summary>
    /// <returns>An enumerable of objects that represent the equality components.</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>
    /// Checks if the current instance is equal to another value object.
    /// </summary>
    /// <param name="other">The other value object to compare.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(ValueObject? other)
    {
        return Equals(other as object);
    }

    /// <summary>
    /// Overrides the default equality check to compare value objects based on their components.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        using var thisValues = GetEqualityComponents().GetEnumerator();
        using var otherValues = other.GetEqualityComponents().GetEnumerator();
        while (thisValues.MoveNext() && otherValues.MoveNext())
        {
            if (ReferenceEquals(thisValues.Current, null) ^ ReferenceEquals(otherValues.Current, null))
            {
                return false;
            }

            if (thisValues.Current != null && !thisValues.Current.Equals(otherValues.Current))
            {
                return false;
            }
        }

        return !thisValues.MoveNext() && !otherValues.MoveNext();
    }

    /// <summary>
    /// Overrides the default hash code generation to use the equality components.
    /// </summary>
    /// <returns>A hash code for the value object.</returns>
    public override int GetHashCode()
    {
        return GetEqualityComponents().Select(x => x.GetHashCode())
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// Checks if two value objects are equal.
    /// </summary>
    /// <param name="left">The left value object.</param>
    /// <param name="right">The right value object.</param>
    /// <returns><c>true</c> if the value objects are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(ValueObject left, ValueObject right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Checks if two value objects are not equal.
    /// </summary>
    /// <param name="left">The left value object.</param>
    /// <param name="right">The right value object.</param>
    /// <returns><c>true</c> if the value objects are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(ValueObject left, ValueObject right) => !(left == right);
}