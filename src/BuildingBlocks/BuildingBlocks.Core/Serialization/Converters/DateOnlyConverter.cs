using System.Globalization;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace BuildingBlocks.Core.Serialization.Converters;

/// <summary>
/// Custom JSON converter for the DateOnly type using the specified format.
/// </summary>
public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";

    /// <summary>
    /// Reads the JSON representation of the DateOnly value.
    /// </summary>
    public override DateOnly ReadJson(
        JsonReader reader,
        Type objectType,
        DateOnly existingValue,
        bool hasExistingValue,
        JsonSerializer serializer
    ) => DateOnly.ParseExact((string)reader.Value, Format, CultureInfo.InvariantCulture);

    /// <summary>
    /// Writes the DateOnly value as a JSON representation.
    /// </summary>
    public override void WriteJson(
        JsonWriter writer,
        DateOnly value,
        JsonSerializer serializer
    ) => writer.WriteValue(value.ToString(Format, CultureInfo.InvariantCulture));
}