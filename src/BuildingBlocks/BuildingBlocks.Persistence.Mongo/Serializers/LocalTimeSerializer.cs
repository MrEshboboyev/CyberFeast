using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace BuildingBlocks.Persistence.Mongo.Serializers;

/// <summary>
/// Provides a serializer for local DateTime values.
/// </summary>
public class LocalDateTimeSerializationProvider : IBsonSerializationProvider
{
    /// <summary>
    /// Gets the serializer for the specified type.
    /// </summary>
    /// <param name="type">The type to get the serializer for.</param>
    /// <returns>The serializer for the specified type, if applicable.</returns>
    public IBsonSerializer? GetSerializer(Type type)
    {
        // Return the local DateTime serializer if the type is DateTime.
        return type == typeof(DateTime) ? DateTimeSerializer.LocalInstance : null;
    }
}