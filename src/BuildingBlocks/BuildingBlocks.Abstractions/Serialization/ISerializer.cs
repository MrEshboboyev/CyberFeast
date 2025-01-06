namespace BuildingBlocks.Abstractions.Serialization;

/// <summary>
/// Defines methods for serializing and deserializing objects.
/// </summary>
public interface ISerializer
{
    /// <summary>
    /// Gets the content type used by the serializer.
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// Serializes the given object into a string.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>A string representation of the serialized object.</returns>
    string Serialize(object obj);

    /// <summary>
    /// Deserializes the given string into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="payload">The string representation of the object.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
    T? Deserialize<T>(string payload);

    /// <summary>
    /// Deserializes the given string into an object of a specified type.
    /// </summary>
    /// <param name="payload">The string representation of the object.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <returns>The deserialized object.</returns>
    object? Deserialize(string payload, Type type);
}