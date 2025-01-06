namespace BuildingBlocks.Abstractions;

/// <summary>
/// Represents a filter condition with a field name, comparison operator, and field value.
/// </summary>
/// <param name="FieldName">The name of the field to filter on.</param>
/// <param name="Comparision">The comparison operator (e.g., "=", ">", "<").</param>
/// <param name="FieldValue">The value to compare the field against.</param>
public record FilterModel(
    string FieldName,
    string Comparision,
    string FieldValue);