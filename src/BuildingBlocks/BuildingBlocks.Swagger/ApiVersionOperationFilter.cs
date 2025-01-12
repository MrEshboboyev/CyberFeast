using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingBlocks.Swagger;

/// <summary>
/// Modifies OpenAPI operations to include API version information.
/// </summary>
public class ApiVersionOperationFilter : IOperationFilter
{
    /// <summary>
    /// Applies the filter to the specified operation.
    /// </summary>
    /// <param name="operation">The OpenAPI operation to modify.</param>
    /// <param name="context">The context for the operation filter.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        operation.Parameters ??= new List<OpenApiParameter>();

        var apiVersionMetadata = actionMetadata.Any(metadataItem => metadataItem is ApiVersionMetadata);
        if (apiVersionMetadata)
        {
            // Uncomment to add version parameters to the operation
            // operation.Parameters.Add(new OpenApiParameter
            // {
            //     Name = Constants.ApiKeyConstants.HeaderVersion,
            //     In = ParameterLocation.Header,
            //     Description = "API Version header value",
            //     Schema = new OpenApiSchema {Type = "String", Default = new OpenApiString("1.0")}
            // });
            // operation.Parameters.Add(new OpenApiParameter
            // {
            //     Name = "{version:apiVersion}",
            //     In = ParameterLocation.Path,
            //     Description = "API Version route value",
            //     Schema = new OpenApiSchema {Type = "String", Default = new OpenApiString("1.0")}
            // });
        }
    }
}