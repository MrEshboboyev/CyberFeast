using BuildingBlocks.Abstractions.Serialization;
using Newtonsoft.Json;

namespace BuildingBlocks.Core.Serialization;

/// <summary>
/// JSON serializer implementation using Newtonsoft.Json.
/// </summary>
public class NewtonsoftObjectSerializer(JsonSerializerSettings settings) : ISerializer
{
    /// <summary>
    /// Gets the content type for the serialized data.
    /// </summary>
    public string ContentType => "application/json";

    /// <summary>
    /// Serializes an object to a JSON string.
    /// </summary>
    public string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj, settings);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of type T.
    /// </summary>
    public T? Deserialize<T>(string payload)
    {
        return JsonConvert.DeserializeObject<T>(payload, settings);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type.
    /// </summary>
    public object? Deserialize(string payload, Type type)
    {
        return JsonConvert.DeserializeObject(payload, type, settings);
    }
}