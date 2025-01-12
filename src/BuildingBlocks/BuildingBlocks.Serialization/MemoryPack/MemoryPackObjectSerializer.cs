using System.Text;
using BuildingBlocks.Abstractions.Serialization;
using MemoryPack;

namespace BuildingBlocks.Serialization.MemoryPack;

/// <summary>
/// Provides methods for serializing and deserializing objects using MemoryPack.
/// </summary>
public class MemoryPackObjectSerializer(MemoryPackSerializerOptions options) : ISerializer
{
    /// <summary>
    /// Gets the content type of the serialized data.
    /// </summary>
    public string ContentType => "binary/memorypack";

    /// <summary>
    /// Serializes an object to a UTF-8 encoded string.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>The serialized object as a string.</returns>
    public string Serialize(object obj)
    {
        return Encoding.UTF8.GetString(MemoryPackSerializer.Serialize(obj, options));
    }

    /// <summary>
    /// Deserializes a string to an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="payload">The serialized object as a string.</param>
    /// <returns>The deserialized object.</returns>
    public T? Deserialize<T>(string payload)
    {
        var byteSpan = StringToReadOnlySpan(payload);
        return MemoryPackSerializer.Deserialize<T>(byteSpan, options);
    }

    /// <summary>
    /// Deserializes a string to an object of the specified type.
    /// </summary>
    /// <param name="payload">The serialized object as a string.</param>
    /// <param name="type">The type of the object.</param>
    /// <returns>The deserialized object.</returns>
    public object? Deserialize(string payload, Type type)
    {
        var byteSpan = StringToReadOnlySpan(payload);
        return MemoryPackSerializer.Deserialize(type, byteSpan, options);
    }

    /// <summary>
    /// Converts a string to a <see cref="ReadOnlySpan{T}"/> of bytes.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>A read-only span of bytes representing the string.</returns>
    private static ReadOnlySpan<byte> StringToReadOnlySpan(string input)
    {
        // Choose the encoding
        var encoding = Encoding.UTF8;

        // Convert the string to a byte array
        var byteArray = encoding.GetBytes(input);

        // Return a ReadOnlySpan<byte> from the byte array
        return new ReadOnlySpan<byte>(byteArray);
    }
}