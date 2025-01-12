using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingBlocks.Swagger;

/// <summary>
/// Customizes the schema for enum types in OpenAPI documentation.
/// </summary>
public class EnumSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Applies the filter to the specified schema.
    /// </summary>
    /// <param name="model">The OpenAPI schema to modify.</param>
    /// <param name="context">The context for the schema filter.</param>
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum)
            return;

        // Clear existing enum values and add the names of the enum members.
        model.Enum.Clear();
        foreach (var n in Enum.GetNames(context.Type))
        {
            model.Enum.Add(new OpenApiString(n));
        }
    }
}